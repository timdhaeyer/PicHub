using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services;

public interface IAlbumRepository
{
    Album? GetByPublicToken(string publicToken);
    void InsertMediaItem(MediaItem item);
}
