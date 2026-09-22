

using RawrPoster.Core.Entites;

namespace RawrPoster.Application.Interfaces
{
    public interface ITelegramPublisher
    {

    Task<long> PublishAsync(
            string channel,
            Post post,
            CancellationToken cancellationToken = default);
    }
}
