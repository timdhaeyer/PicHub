using System;
using System.Linq;
using PicHub.AlbumUploader.Services.Cqrs.Queries;
using PicHub.AlbumUploader.Tests.Support;
using PicHub.AlbumUploader.Models;
using Xunit;

namespace PicHub.AlbumUploader.Tests
{
    public class GetPublicAlbumHandlerEdgeCasesTests
    {
        [Fact]
        public async System.Threading.Tasks.Task Handle_Returns_Null_When_Album_NotFound()
        {
            var repo = new TestAlbumRepository();
            var blob = new PicHub.AlbumUploader.Services.Storage.InMemoryBlobService();
            var handler = new PicHub.AlbumUploader.Services.Cqrs.Queries.GetPublicAlbumHandler(repo, blob, Microsoft.Extensions.Logging.Abstractions.NullLogger<PicHub.AlbumUploader.Services.Cqrs.Queries.GetPublicAlbumHandler>.Instance);

            var result = await handler.Handle(new GetPublicAlbumQuery("no-such"), default);
            Assert.Null(result);
        }

        [Fact]
        public async System.Threading.Tasks.Task Handle_Encodes_StoragePath_With_Spaces_And_Slashes()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Enc",
                PublicToken = "enc-1",
                AllowUploads = true
            };
            repo.InsertAlbum(album);

            var item = new MediaItem
            {
                Id = Guid.NewGuid(),
                AlbumId = album.Id,
                Filename = "img 1.jpg",
                SizeBytes = 100,
                UploadedAt = DateTime.UtcNow,
                StoragePath = "path with spaces/and/slash.jpg"
            };
            repo.InsertMediaItem(item);

            var blob = new PicHub.AlbumUploader.Services.Storage.InMemoryBlobService();
            var handler = new PicHub.AlbumUploader.Services.Cqrs.Queries.GetPublicAlbumHandler(repo, blob, Microsoft.Extensions.Logging.Abstractions.NullLogger<PicHub.AlbumUploader.Services.Cqrs.Queries.GetPublicAlbumHandler>.Instance);
            var result = await handler.Handle(new GetPublicAlbumQuery("enc-1"), default);

            Assert.NotNull(result);
            Assert.Single(result!.Items);
            var uri = result.Items.First().BlobUri;
            Assert.NotNull(uri);
            // Relative URI should contain encoded spaces as %20 but keep slashes
            var s = uri!.ToString();
            Assert.Contains("path%20with%20spaces/and/slash.jpg", s);
            Assert.StartsWith("/api/blobs/", s);
        }
    }
}
