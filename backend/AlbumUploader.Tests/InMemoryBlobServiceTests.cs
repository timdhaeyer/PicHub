using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PicHub.AlbumUploader.Services.Storage;
using Xunit;

namespace PicHub.AlbumUploader.Tests
{
    public class InMemoryBlobServiceTests
    {
        [Fact]
        public async Task Upload_Then_OpenRead_Returns_SameBytes()
        {
            var svc = new InMemoryBlobService();
            var container = "test-container";
            var blob = "path/to/blob.bin";

            var payload = Encoding.UTF8.GetBytes("hello world");
            using var ms = new MemoryStream(payload);

            await svc.UploadAsync(container, blob, ms);

            var uri = await svc.GetBlobUriAsync(container, blob);
            Assert.NotNull(uri);
            Assert.Equal(new Uri($"inmemory://{container}/{blob}"), uri);

            using var r = await svc.OpenReadAsync(container, blob);
            using var outMs = new MemoryStream();
            await r.CopyToAsync(outMs);
            var got = outMs.ToArray();
            Assert.Equal(payload, got);
        }

        [Fact]
        public async Task OpenRead_Throws_When_NotFound()
        {
            var svc = new InMemoryBlobService();
            await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            {
                await svc.OpenReadAsync("no", "such");
            });
        }
    }
}
