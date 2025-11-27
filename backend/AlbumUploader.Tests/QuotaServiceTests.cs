using System;
using AlbumUploader.Models;
using AlbumUploader.Services;
using Xunit;

namespace AlbumUploader.Tests
{
    public class QuotaServiceTests
    {
        [Fact]
        public void CanUpload_RespectsAlbumCap()
        {
            var album = new Album
            {
                Id = Guid.NewGuid(),
                AllowUploads = true,
                AlbumSizeTshirt = "XS",
                TotalBytesUsed = 0
            };

            var svc = new QuotaService();

            // XS = 1GB
            var can = svc.CanUpload(album, 512 * 1024 * 1024); // 512MB
            Assert.True(can);

            var cannot = svc.CanUpload(album, 2L * 1024 * 1024 * 1024); // 2GB
            Assert.False(cannot);
        }
    }
}
