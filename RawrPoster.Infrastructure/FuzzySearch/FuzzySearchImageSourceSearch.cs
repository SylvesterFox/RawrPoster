using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RawrPoster.Application.Interfaces;

namespace RawrPoster.Infrastructure.FuzzySearch;

public sealed class FuzzySearchImageSourceSearch(HttpClient httpClient, ILogger<FuzzySearchImageSourceSearch> logger) : IImageSourceSearch
{
    private const string Endpoint = "https://api-next.fuzzysearch.net/v1/image";

    public async Task<IReadOnlyList<ImageSearchResult>> SearchAsync(Stream image, string fileName, CancellationToken cancellationToken = default)
    {
        var apiKey = Environment.GetEnvironmentVariable("RAWRPOSTER_FUZZYSEARCH_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey)) throw new InvalidOperationException("FuzzySearch API key is not configured.");

        using var body = new MultipartFormDataContent();
        using var file = new StreamContent(image);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        body.Add(file, "image", fileName);
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint) { Content = body };
        request.Headers.Add("x-api-key", apiKey);
        logger.LogInformation("Starting FuzzySearch image lookup.");
        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("FuzzySearch returned HTTP {StatusCode}.", (int)response.StatusCode);
            throw new FuzzySearchException(response.StatusCode);
        }

        await using var json = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(json, cancellationToken: cancellationToken);
        var array = document.RootElement.ValueKind == JsonValueKind.Array
            ? document.RootElement : document.RootElement.TryGetProperty("results", out var results) ? results : default;
        if (array.ValueKind != JsonValueKind.Array) return [];
        return array.EnumerateArray().Select(Map).ToArray();
    }

    private static ImageSearchResult Map(JsonElement item)
    {
        static string? String(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
        static double? Number(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.TryGetDouble(out var n) ? n : null;
        static IReadOnlyList<string> Strings(JsonElement e, string key) => e.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.Array
            ? v.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).ToArray() : [];
        return new ImageSearchResult(String(item, "site") ?? "Unknown", String(item, "postId"), String(item, "postUrl"), Number(item, "distance"), Strings(item, "tags"), Strings(item, "sources"), String(item, "previewUrl") ?? String(item, "thumbnailUrl"));
    }
}

public sealed class FuzzySearchException(HttpStatusCode statusCode) : Exception(UserMessage(statusCode))
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    private static string UserMessage(HttpStatusCode code) => code switch
    {
        HttpStatusCode.BadRequest => "FuzzySearch rejected the image.",
        HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden => "FuzzySearch authorization failed.",
        HttpStatusCode.RequestEntityTooLarge => "The image is too large for FuzzySearch.",
        (HttpStatusCode)429 => "FuzzySearch rate limit reached. Please try again later.",
        _ => "Image source search failed."
    };
}
