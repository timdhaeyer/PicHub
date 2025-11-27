using System;
using PicHub.AlbumUploader.Models;
using PicHub.AlbumUploader.Services;
using Xunit;

namespace PicHub.AlbumUploader.Tests
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

            // XS = 1GB
            var can = QuotaService.CanUpload(album, 512 * 1024 * 1024); // 512MB
            Assert.True(can);

            var cannot = QuotaService.CanUpload(album, 2L * 1024 * 1024 * 1024); // 2GB
            Assert.False(cannot);
        }
    }
}
