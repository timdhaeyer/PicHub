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

    public Album? GetByTitle(string title)
    {
        return _db.Albums.FirstOrDefault(a => a.Title == title);
    }

    public IEnumerable<Models.MediaItem> GetMediaItems(Guid albumId)
    {
        return _db.MediaItems.Where(m => m.AlbumId == albumId).ToList();
    }

    public Models.MediaItem? GetMediaItemByStoragePath(string storagePath)
    {
        return _db.MediaItems.FirstOrDefault(m => m.StoragePath == storagePath);
    }

    public void InsertAlbum(Album album)
    {
        _db.Albums.Add(album);
        _db.SaveChanges();
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
