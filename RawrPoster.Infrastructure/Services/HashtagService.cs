using Microsoft.EntityFrameworkCore;
using RawrPoster.Application.Interfaces;
using RawrPoster.Core.Entites;
using RawrPoster.Infrastructure.Persistence;

namespace RawrPoster.Infrastructure.Services;

public sealed class HashtagService(RawrPosterDbContext db) : IHashtagService
{
    public async Task<IReadOnlyList<Hashtag>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var value = Normalize(query);
        return await db.Hashtags.Where(x => x.Value.StartsWith(value)).OrderBy(x => x.Value).Take(8).ToListAsync(cancellationToken);
    }

    public async Task<Hashtag> SaveAsync(string value, CancellationToken cancellationToken = default)
    {
        value = Normalize(value);
        if (value == "#") throw new ArgumentException("Hashtag is empty.", nameof(value));
        var existing = await db.Hashtags.SingleOrDefaultAsync(x => x.Value == value, cancellationToken);
        if (existing is not null) return existing;
        var tag = new Hashtag { Value = value };
        db.Hashtags.Add(tag);
        await db.SaveChangesAsync(cancellationToken);
        return tag;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await db.Hashtags.FindAsync([id], cancellationToken);
        if (tag is null) return;
        db.Hashtags.Remove(tag);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static string Normalize(string value) => "#" + value.Trim().TrimStart('#').ToLowerInvariant();
}
