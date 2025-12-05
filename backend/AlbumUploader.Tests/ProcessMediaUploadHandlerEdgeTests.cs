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
    public class ProcessMediaUploadHandlerEdgeTests
    {
        [Fact]
        public async Task UploadRejectsLargeFile_Test()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "LargeLimit",
                PublicToken = "t1",
                AllowUploads = true,
                MaxFileSizeMb = 1 // 1 MB
            };
            repo.InsertAlbum(album);

            var blob = new InMemoryBlobService();

            // Create payload ~2MB
            var boundary = "----BoundaryLarge";
            var sb = new StringBuilder();
            sb.AppendLine($"--{boundary}");
            sb.AppendLine("Content-Disposition: form-data; name=\"file\"; filename=\"big.bin\"");
            sb.AppendLine("Content-Type: image/png");
            sb.AppendLine();
            // write 2 * 1024 * 1024 bytes
            var large = new byte[2 * 1024 * 1024];
            for (int i = 0; i < large.Length; i++) large[i] = 0x20;
            var header = Encoding.UTF8.GetBytes(sb.ToString());
            var footer = Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
            using var ms = new MemoryStream();
            ms.Write(header, 0, header.Length);
            ms.Write(large, 0, large.Length);
            ms.Write(footer, 0, footer.Length);
            ms.Seek(0, SeekOrigin.Begin);

            var cmd = new ProcessMediaUploadCommand(album, ms, $"multipart/form-data; boundary={boundary}");
            var handler = new ProcessMediaUploadHandler(repo, blob);
            var result = await handler.Handle(cmd, default);

            Assert.False(result.Success);
            Assert.Equal(System.Net.HttpStatusCode.RequestEntityTooLarge, result.ErrorStatusCode);
        }

        [Fact]
        public async Task UploadRejectsDisallowedContentType_Test()
        {
            var repo = new TestAlbumRepository();
            var album = new Album
            {
                Id = Guid.NewGuid(),
                Title = "Disallowed",
                PublicToken = "t2",
                AllowUploads = true,
                MaxFileSizeMb = 5
            };
            repo.InsertAlbum(album);

            var blob = new InMemoryBlobService();

            var boundary = "----BoundaryCT";
            var sb = new StringBuilder();
            sb.AppendLine($"--{boundary}");
            sb.AppendLine("Content-Disposition: form-data; name=\"file\"; filename=\"malware.bin\"");
            sb.AppendLine("Content-Type: application/x-msdownload");
            sb.AppendLine();
            sb.AppendLine("not an image");
            sb.AppendLine($"--{boundary}--");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            using var ms = new MemoryStream(bytes);

            var cmd = new ProcessMediaUploadCommand(album, ms, $"multipart/form-data; boundary={boundary}");
            var handler = new ProcessMediaUploadHandler(repo, blob);
            var result = await handler.Handle(cmd, default);

            Assert.False(result.Success);
            Assert.Equal(System.Net.HttpStatusCode.UnsupportedMediaType, result.ErrorStatusCode);
        }
    }
}
