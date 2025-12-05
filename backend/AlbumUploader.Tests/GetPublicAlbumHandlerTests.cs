using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using PicHub.AlbumUploader.Services.Cqrs.Queries;
using PicHub.AlbumUploader.Tests.Support;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Tests;

public class GetPublicAlbumHandlerTests
{
    [Fact]
    public async Task Handle_Returns_PublicAlbumDto_With_Items()
    {
        var repo = new TestAlbumRepository();
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Title = "Public Album",
            PublicToken = "public-123",
            AllowUploads = true,
            MaxFileSizeMb = 10
        };
        repo.InsertAlbum(album);

        var item = new MediaItem
        {
            Id = Guid.NewGuid(),
            AlbumId = album.Id,
            Filename = "pic.jpg",
            SizeBytes = 12345,
            UploadedAt = DateTime.UtcNow,
            StoragePath = "a/b/c"
        };
        repo.InsertMediaItem(item);

        var blob = new PicHub.AlbumUploader.Services.Storage.InMemoryBlobService();
        var handler = new GetPublicAlbumHandler(repo, blob);
        var result = await handler.Handle(new GetPublicAlbumQuery("public-123"), default);

        Assert.NotNull(result);
        Assert.Equal(album.Id, result!.Id);
        Assert.Single(result.Items);
        var returned = result.Items.First();
        Assert.Equal(item.Id, returned.Id);
        Assert.NotNull(returned.BlobUri);
    }
}
