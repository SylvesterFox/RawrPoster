using Microsoft.EntityFrameworkCore;
using RawrPoster.Core.Entites;

namespace RawrPoster.Infrastructure.Persistence;

public sealed class RawrPosterDbContext(DbContextOptions<RawrPosterDbContext> options) : DbContext(options)
{
    public DbSet<PostTemplate> Templates => Set<PostTemplate>();
    public DbSet<Hashtag> Hashtags => Set<Hashtag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostTemplate>(entity =>
        {
            entity.ToTable("Templates");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(160).IsRequired();
            entity.Property(x => x.Text).IsRequired();
            entity.OwnsOne(x => x.Media, media =>
            {
                media.Property(x => x.LocalPath).HasMaxLength(2048);
                media.Property(x => x.SourceUrl).HasMaxLength(2048);
                media.Property(x => x.FileName).HasMaxLength(512);
                media.Property(x => x.MimeType).HasMaxLength(160);
            });
            entity.HasMany(x => x.Hashtags).WithMany(x => x.Templates)
                .UsingEntity("TemplateHashtags");
        });

        modelBuilder.Entity<Hashtag>(entity =>
        {
            entity.ToTable("Hashtags");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).HasMaxLength(128).IsRequired();
            entity.HasIndex(x => x.Value).IsUnique();
        });
    }
}
