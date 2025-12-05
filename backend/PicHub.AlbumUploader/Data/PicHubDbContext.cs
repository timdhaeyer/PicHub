using Microsoft.EntityFrameworkCore;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Data;

public class PicHubDbContext(DbContextOptions<PicHubDbContext> options) : DbContext(options)
{
    public DbSet<Album> Albums { get; set; } = null!;
    public DbSet<MediaItem> MediaItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(eb =>
        {
            eb.ToTable("Albums");
            eb.HasKey(a => a.Id);
            eb.HasIndex(a => a.PublicToken).HasDatabaseName("IDX_Albums_PublicToken");
        });

        modelBuilder.Entity<MediaItem>(mb =>
        {
            mb.ToTable("MediaItems");
            mb.HasKey(m => m.Id);
            mb.HasIndex(m => m.AlbumId).HasDatabaseName("IDX_MediaItems_AlbumId");
        });
    }
}
