using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services.Storage;
using PicHub.AlbumUploader.Tests.Support;
using PicHub.AlbumUploader.Models;
using Xunit;

namespace PicHub.AlbumUploader.Tests
{
    public class ProcessMediaUploadHandlerValidationTests
    {
        [Fact]
        public async Task UploadRejectsLargeFile_Test()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Big",
                PublicToken = "big-1",
                AllowUploads = true,
                MaxFileSizeMb = 1 // 1 MB
            };
            repo.InsertAlbum(album);

            var blob = new InMemoryBlobService();

            // Create ~2 MB content
            var bytes = new byte[2 * 1024 * 1024];
            new Random(42).NextBytes(bytes);
            using var ms = new MemoryStream(bytes);

            var boundary = "----Boundary";
            var pre = $"--{boundary}\r\nContent-Disposition: form-data; name=\"file\"; filename=\"big.bin\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            var post = $"\r\n--{boundary}--\r\n";

            using var combined = new MemoryStream();
            var preBytes = Encoding.UTF8.GetBytes(pre);
            var postBytes = Encoding.UTF8.GetBytes(post);
            await combined.WriteAsync(preBytes, 0, preBytes.Length);
            ms.Position = 0;
            await ms.CopyToAsync(combined);
            await combined.WriteAsync(postBytes, 0, postBytes.Length);
            combined.Position = 0;
            using var bodyStream = combined;
            var cmd = new ProcessMediaUploadCommand(album, bodyStream, $"multipart/form-data; boundary={boundary}");
            var handler = new ProcessMediaUploadHandler(repo, blob);
            var result = await handler.Handle(cmd, default);

            Assert.False(result.Success);
            Assert.Contains("exceed", result.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UploadRejectsDisallowedContentType_Test()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Bad",
                PublicToken = "bad-1",
                AllowUploads = true,
                MaxFileSizeMb = 10
            };
            repo.InsertAlbum(album);

            var blob = new InMemoryBlobService();

            var boundary = "----Boundary2";
            var sb = new StringBuilder();
            sb.Append($"--{boundary}")
                .Append("\r\n")
                .Append("Content-Disposition: form-data; name=\"file\"; filename=\"evil.exe\"")
                .Append("\r\n")
                .Append("Content-Type: application/x-msdownload")
                .Append("\r\n\r\n")
                .Append("MZ...binary...")
                .Append("\r\n")
                .Append($"--{boundary}--")
                .Append("\r\n");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            using var ms = new MemoryStream(bytes);

            var cmd = new ProcessMediaUploadCommand(album, ms, $"multipart/form-data; boundary={boundary}");
            var handler = new ProcessMediaUploadHandler(repo, blob);
            var result = await handler.Handle(cmd, default);

            Assert.False(result.Success);
            Assert.Contains("not allowed", result.ErrorMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }
    }
}
