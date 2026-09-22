
using RawrPoster.Core.Enums;

namespace RawrPoster.Core.Entites
{
    public sealed class Post
    {
       
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Text { get; set; } = string.Empty;

        public MediaAttachment? Media { get; set; }

        public bool HasSpoiler { get; set; }

        public PostStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? PublishedAt { get; set; }

        public long? TelegramMessageId { get; set; }
    }
}
