using System;
using Xunit;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Tests;

public class CreateAlbumHandlerTests
{
    private class InMemoryAlbumRepo : IAlbumRepository
    {
        public Album? LastInserted { get; private set; }
        public void InsertAlbum(Album album)
        {
            LastInserted = album;
        }

        public Album? GetByPublicToken(string publicToken) => null;
        public Album? GetByTitle(string title) => null;
        public void InsertMediaItem(MediaItem item) => throw new NotImplementedException();
    }

    [Fact]
    public void CreateAlbumHandler_MapsFieldsToAlbum()
    {
        var repo = new InMemoryAlbumRepo();
        var handler = new PicHub.AlbumUploader.Services.Cqrs.Commands.CreateAlbumHandler(repo);

        var cmd = new CreateAlbumCommand(
            Title: "My Test",
            Description: "Desc",
            AllowUploads: false,
            MaxFileSizeMb: 25,
            AlbumSizeTshirt: "S",
            RetentionDays: 7
        );

        var result = handler.Handle(cmd, default).GetAwaiter().GetResult();

        Assert.NotNull(repo.LastInserted);
        var inserted = repo.LastInserted!;
        Assert.Equal(cmd.Title, inserted.Title);
        Assert.Equal(cmd.Description, inserted.Description);
        Assert.Equal(cmd.AllowUploads, inserted.AllowUploads);
        Assert.Equal(cmd.MaxFileSizeMb, inserted.MaxFileSizeMb);
        Assert.Equal(cmd.AlbumSizeTshirt, inserted.AlbumSizeTshirt);
    }
}
