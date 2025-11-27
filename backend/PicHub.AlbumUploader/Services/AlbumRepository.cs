using PicHub.AlbumUploader.Data;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services;

public class AlbumRepository(PicHubDbContext db) : IAlbumRepository
{
    private readonly PicHubDbContext _db = db;

    public Album? GetByPublicToken(string publicToken)
    {
        return _db.Albums.FirstOrDefault(a => a.PublicToken == publicToken);
    }

    public void InsertMediaItem(MediaItem item)
    {
        _db.MediaItems.Add(item);
        var album = _db.Albums.Find(item.AlbumId);
        if (album != null)
        {
            album.TotalBytesUsed = album.TotalBytesUsed + item.SizeBytes;
        }
        _db.SaveChanges();
    }
}
