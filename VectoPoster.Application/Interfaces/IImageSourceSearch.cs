namespace RawrPoster.Application.Interfaces;

public interface IImageSourceSearch
{
    Task<IReadOnlyList<ImageSearchResult>> SearchAsync(Stream image, string fileName, CancellationToken cancellationToken = default);
}

public sealed record ImageSearchResult(string Site, string? PostId, string? PostUrl, double? Distance, IReadOnlyList<string> Tags, IReadOnlyList<string> Sources, string? PreviewUrl);
