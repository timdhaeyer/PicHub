using System;
using System.Linq;
using System.Collections.Generic;
using PicHub.AlbumUploader.Models;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services;
using Xunit;

namespace PicHub.AlbumUploader.Tests
{
    public class CreateAlbumHandlerAdditionalTests
    {
        private class InMemoryAlbumRepo : IAlbumRepository
        {
            public Album? LastInserted { get; private set; }
            public void InsertAlbum(Album album) => LastInserted = album;
            public Album? GetByPublicToken(string publicToken) => null;
            public Album? GetByTitle(string title) => null;
            public void InsertMediaItem(MediaItem item) => throw new NotImplementedException();
            public IEnumerable<MediaItem> GetMediaItems(Guid albumId) => Array.Empty<MediaItem>();
            public MediaItem? GetMediaItemByStoragePath(string storagePath) => null;
            public Album? GetById(Guid id) => LastInserted != null && LastInserted.Id == id ? LastInserted : null;
        }

        [Fact]
        public void CreateAlbum_Defaults_AlbumSizeTshirt_To_M_When_Null()
        {
            var repo = new InMemoryAlbumRepo();
            var handler = new PicHub.AlbumUploader.Services.Cqrs.Commands.CreateAlbumHandler(repo);

            var cmd = new CreateAlbumCommand(
                Title: "T1",
                Description: "",
                AllowUploads: true,
                MaxFileSizeMb: 5,
                AlbumSizeTshirt: null
            );

            var result = handler.Handle(cmd, default).GetAwaiter().GetResult();
            Assert.NotNull(repo.LastInserted);
            Assert.Equal("M", repo.LastInserted!.AlbumSizeTshirt);
            Assert.False(string.IsNullOrEmpty(result.PublicToken));
        }

        [Fact]
        public void CreateAlbum_Generates_Unique_PublicTokens()
        {
            var repo = new InMemoryAlbumRepo();
            var handler = new PicHub.AlbumUploader.Services.Cqrs.Commands.CreateAlbumHandler(repo);

            var a = handler.Handle(new CreateAlbumCommand("A", "", false, 1, null), default).GetAwaiter().GetResult();
            var b = handler.Handle(new CreateAlbumCommand("B", "", false, 1, null), default).GetAwaiter().GetResult();

            Assert.NotEqual(a.PublicToken, b.PublicToken);
        }
    }
}
