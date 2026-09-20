

using RawrPoster.Core.Entites;
using RawrPoster.Core.Enums;
using RawrPoster.Application.Interfaces;

namespace RawrPoster.Application.Services
{
    public sealed class PostService
    {
        private readonly ITelegramPublisher _telegramPublisher;

        public PostService(ITelegramPublisher telegramPublisher)
        {
            _telegramPublisher = telegramPublisher;
        }

        public async Task<long> PublishAsync(
            string channel,
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(channel))
                throw new ArgumentException(
                    "Telegram channel is required.",
                    nameof(channel));

            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException(
                    "Post text is required.",
                    nameof(text));

            var post = new Post
            {
                Id = Guid.NewGuid(),
                Text = text,
                Status = PostStatus.Publishing,
                CreatedAt = DateTimeOffset.UtcNow
            };

            try
            {
                var messageId = await _telegramPublisher.PublishAsync(
                    channel,
                    post,
                    cancellationToken);

                post.Status = PostStatus.Published;
                post.PublishedAt = DateTimeOffset.UtcNow;
                post.TelegramMessageId = messageId;

                return messageId;
            }
            catch
            {
                post.Status = PostStatus.Failed;
                throw;
            }
        }
    }
}
