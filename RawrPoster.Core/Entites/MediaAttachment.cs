namespace RawrPoster.Core.Entites;

public sealed class MediaAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string LocalPath { get; set; } = string.Empty;
    public string? SourceUrl { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? MimeType { get; set; }
}
