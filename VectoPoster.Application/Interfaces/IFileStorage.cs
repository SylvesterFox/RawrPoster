using RawrPoster.Core.Entites;

namespace RawrPoster.Application.Interfaces;

public interface IFileStorage
{
    Task<MediaAttachment> SaveImageAsync(Stream content, string fileName, string? mimeType, CancellationToken cancellationToken = default);
}
