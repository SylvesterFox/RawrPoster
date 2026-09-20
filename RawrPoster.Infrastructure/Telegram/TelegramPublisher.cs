
using RawrPoster.Core.Entites;
using Telegram.Bot;


namespace Infrastructure.Telegram
{
    public sealed class TelegramPublisher
    {
        private readonly TelegramBotClient _bot;

        public TelegramPublisher(TelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task<long> PublishAsync(
            string channel,
            Post post,
            CancellationToken cancellationToken = default)
        {
            var message = await _bot.SendMessage(
                chatId: channel,
                text: post.Text,
                cancellationToken: cancellationToken);

            return message.Id;
        }
    }
}
