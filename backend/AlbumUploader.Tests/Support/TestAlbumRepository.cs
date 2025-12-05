#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PicHub.AlbumUploader.Models;
using PicHub.AlbumUploader.Services;

namespace PicHub.AlbumUploader.Tests.Support;

public class TestAlbumRepository : IAlbumRepository
{
    private readonly ConcurrentDictionary<Guid, Album> _albums = new();
    private readonly ConcurrentDictionary<Guid, MediaItem> _media = new();

    public Album? GetByPublicToken(string publicToken)
    {
        return _albums.Values.FirstOrDefault(a => a.PublicToken == publicToken);
    }

    public Album? GetByTitle(string title)
    {
        return _albums.Values.FirstOrDefault(a => string.Equals(a.Title, title, StringComparison.OrdinalIgnoreCase));
    }

    public void InsertAlbum(Album album)
    {
        _albums[album.Id] = album;
    }

    public void InsertMediaItem(MediaItem item)
    {
        _media[item.Id] = item;
        if (_albums.TryGetValue(item.AlbumId, out var a))
        {
            a.TotalBytesUsed = a.TotalBytesUsed + item.SizeBytes;
        }
    }

    public IEnumerable<MediaItem> GetMediaItems(Guid albumId)
    {
        return _media.Values.Where(m => m.AlbumId == albumId).ToList();
    }
}
