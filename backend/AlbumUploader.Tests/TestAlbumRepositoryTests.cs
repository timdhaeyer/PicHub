using System;
using System.Linq;
using PicHub.AlbumUploader.Models;
using PicHub.AlbumUploader.Tests.Support;
using Xunit;

namespace PicHub.AlbumUploader.Tests
{
    public class TestAlbumRepositoryTests
    {
        [Fact]
        public void InsertAlbum_And_GetByPublicToken_Works()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "RepoTest",
                PublicToken = "ptok"
            };
            repo.InsertAlbum(album);

            var got = repo.GetByPublicToken("ptok");
            Assert.NotNull(got);
            Assert.Equal(album.Id, got!.Id);
        }

        [Fact]
        public void InsertMediaItem_Updates_TotalBytesUsed_And_GetMediaItems()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "RepoMedia",
                PublicToken = "mptok",
                TotalBytesUsed = 0
            };
            repo.InsertAlbum(album);

            var item = new MediaItem
            {
                Id = Guid.NewGuid(),
                AlbumId = album.Id,
                Filename = "one.jpg",
                SizeBytes = 500
            };
            repo.InsertMediaItem(item);

            var items = repo.GetMediaItems(album.Id).ToList();
            Assert.Single(items);
            Assert.Equal(item.Id, items[0].Id);

            var updated = repo.GetByPublicToken("mptok");
            Assert.NotNull(updated);
            Assert.Equal(500, updated!.TotalBytesUsed);
        }

        [Fact]
        public void GetMediaItemByStoragePath_Finds_Item()
        {
            var repo = new TestAlbumRepository();
            var album = new Album { Id = Guid.NewGuid(), Title = "SPath", PublicToken = "sp" };
            repo.InsertAlbum(album);

            var item = new MediaItem
            {
                Id = Guid.NewGuid(),
                AlbumId = album.Id,
                Filename = "f.jpg",
                StoragePath = "store/1",
                SizeBytes = 10
            };
            repo.InsertMediaItem(item);

            var found = repo.GetMediaItemByStoragePath("store/1");
            Assert.NotNull(found);
            Assert.Equal(item.Id, found!.Id);
        }
    }
}
