using RawrPoster.Core.Entites;

namespace RawrPoster.Application.Interfaces;

public interface IHashtagService
{
    Task<IReadOnlyList<Hashtag>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<Hashtag> SaveAsync(string value, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
