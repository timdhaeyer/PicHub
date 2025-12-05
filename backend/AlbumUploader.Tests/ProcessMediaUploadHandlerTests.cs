using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services.Storage;
using PicHub.AlbumUploader.Tests.Support;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Tests;

public class ProcessMediaUploadHandlerTests
{
    [Fact]
    public async Task Handle_Uploads_File_When_Album_Allows_Uploads()
    {
        var repo = new TestAlbumRepository();
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Title = "Public Album",
            PublicToken = "pub-1",
            AllowUploads = true,
            MaxFileSizeMb = 5
        };
        repo.InsertAlbum(album);

        var blob = new InMemoryBlobService();

        // Build a simple multipart body with one file
        var boundary = "----WebKitFormBoundary7MA4YWxkTrZu0gW";
        var content = new StringBuilder();
        content.AppendLine($"--{boundary}");
        content.AppendLine("Content-Disposition: form-data; name=\"file\"; filename=\"test.txt\"");
        content.AppendLine("Content-Type: image/png");
        content.AppendLine();
        content.AppendLine("hello world");
        content.AppendLine($"--{boundary}--");

        var bytes = Encoding.UTF8.GetBytes(content.ToString());
        using var ms = new MemoryStream(bytes);

        var cmd = new ProcessMediaUploadCommand(album, ms, $"multipart/form-data; boundary={boundary}");
        var handler = new ProcessMediaUploadHandler(repo, blob);
        var result = await handler.Handle(cmd, default);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.NotNull(result.UploadedItems);
        Assert.Single(result.UploadedItems);
    }
}
