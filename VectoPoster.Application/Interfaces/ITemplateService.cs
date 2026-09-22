using RawrPoster.Core.Entites;

namespace RawrPoster.Application.Interfaces;

public interface ITemplateService
{
    Task<IReadOnlyList<PostTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PostTemplate?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PostTemplate> SaveAsync(PostTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
