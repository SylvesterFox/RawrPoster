namespace RawrPoster.Core.Entites;

public sealed class Hashtag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Value { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public ICollection<PostTemplate> Templates { get; set; } = new List<PostTemplate>();
}
