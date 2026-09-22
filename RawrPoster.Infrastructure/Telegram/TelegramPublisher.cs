
using RawrPoster.Core.Entites;
using RawrPoster.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace RawrPoster.Infrastructure.Telegram
{
    public sealed class TelegramPublisher : ITelegramPublisher
    {
        public async Task<long> PublishAsync(
            string channel,
            Post post,
            CancellationToken cancellationToken = default)
        {
            var token = Environment.GetEnvironmentVariable("RAWRPOSTER_TELEGRAM_BOT_TOKEN");
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Telegram bot token is not configured.");
            var bot = new TelegramBotClient(token);
            if (post.Media is { LocalPath: { Length: > 0 } path } && File.Exists(path))
            {
                await using var stream = File.OpenRead(path);
                var photoMessage = await bot.SendPhoto(
                    chatId: channel,
                    photo: InputFile.FromStream(stream, post.Media.FileName),
                    caption: post.Text,
                    hasSpoiler: post.HasSpoiler,
                    cancellationToken: cancellationToken);
                return photoMessage.Id;
            }

            var message = await bot.SendMessage(chatId: channel, text: post.Text, cancellationToken: cancellationToken);

            return message.Id;
        }
    }
}
