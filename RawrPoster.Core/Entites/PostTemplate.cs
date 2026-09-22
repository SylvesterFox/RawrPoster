namespace RawrPoster.Core.Entites;

public sealed class PostTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public MediaAttachment? Media { get; set; }
    public bool HasSpoiler { get; set; }
    public ICollection<Hashtag> Hashtags { get; set; } = new List<Hashtag>();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
