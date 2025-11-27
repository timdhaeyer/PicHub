using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services;

public interface IAlbumRepository
{
    Album? GetByPublicToken(string publicToken);
    Album? GetByTitle(string title);
    void InsertAlbum(Album album);
    void InsertMediaItem(MediaItem item);
}
