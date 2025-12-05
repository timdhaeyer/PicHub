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

        // Build a simple multipart body with one file (use CRLF explicitly)
        var boundary = "----WebKitFormBoundary7MA4YWxkTrZu0gW";
        var sb = new StringBuilder();
        sb.Append($"--{boundary}")
            .Append("\r\n")
            .Append("Content-Disposition: form-data; name=\"file\"; filename=\"test.txt\"")
            .Append("\r\n")
            .Append("Content-Type: image/png")
            .Append("\r\n\r\n")
            .Append("hello world")
            .Append("\r\n")
            .Append($"--{boundary}--")
            .Append("\r\n");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        using var ms = new MemoryStream(bytes);

        var cmd = new ProcessMediaUploadCommand(album, ms, $"multipart/form-data; boundary={boundary}");
        var handler = new ProcessMediaUploadHandler(repo, blob);
        var result = await handler.Handle(cmd, default);

        Assert.True(result.Success, result.ErrorMessage);
        Assert.NotNull(result.UploadedItems);
        Assert.Single(result.UploadedItems);
    }
}
