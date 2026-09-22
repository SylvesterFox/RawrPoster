using RawrPoster.Application.Interfaces;
using RawrPoster.Core.Entites;

namespace RawrPoster.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _mediaDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RawrPoster", "media");

    public async Task<MediaAttachment> SaveImageAsync(Stream content, string fileName, string? mimeType, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_mediaDirectory);
        var safeName = Path.GetFileName(fileName);
        var path = Path.Combine(_mediaDirectory, $"{Guid.NewGuid():N}_{safeName}");
        await using var destination = File.Create(path);
        await content.CopyToAsync(destination, cancellationToken);
        return new MediaAttachment { LocalPath = path, FileName = safeName, MimeType = mimeType };
    }
}
