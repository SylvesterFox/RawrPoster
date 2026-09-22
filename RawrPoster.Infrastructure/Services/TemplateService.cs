using Microsoft.EntityFrameworkCore;
using RawrPoster.Application.Interfaces;
using RawrPoster.Core.Entites;
using RawrPoster.Infrastructure.Persistence;

namespace RawrPoster.Infrastructure.Services;

public sealed class TemplateService(RawrPosterDbContext db) : ITemplateService
{
    public async Task<IReadOnlyList<PostTemplate>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Templates.Include(x => x.Hashtags).OrderByDescending(x => x.UpdatedAt).ToListAsync(cancellationToken);

    public Task<PostTemplate?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Templates.Include(x => x.Hashtags).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PostTemplate> SaveAsync(PostTemplate template, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(template.Name)) throw new ArgumentException("Template name is required.", nameof(template));
        template.UpdatedAt = DateTimeOffset.UtcNow;
        var ids = template.Hashtags.Select(x => x.Id).Where(x => x != Guid.Empty).ToArray();
        var tags = await db.Hashtags.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        var existing = await db.Templates.Include(x => x.Hashtags).SingleOrDefaultAsync(x => x.Id == template.Id, cancellationToken);
        if (existing is null)
        {
            template.Hashtags = tags;
            db.Templates.Add(template);
            await db.SaveChangesAsync(cancellationToken);
            return template;
        }

        existing.Name = template.Name;
        existing.Text = template.Text;
        existing.Media = template.Media;
        existing.HasSpoiler = template.HasSpoiler;
        existing.UpdatedAt = template.UpdatedAt;
        existing.Hashtags = tags;
        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await db.Templates.FindAsync([id], cancellationToken);
        if (template is null) return;
        db.Templates.Remove(template);
        await db.SaveChangesAsync(cancellationToken);
    }
}
