using PicHub.AlbumUploader.Services;
using Xunit;

namespace AlbumUploader.Tests
{
    public class FileValidationServiceTests
    {
        [Theory]
        [InlineData("image/jpeg", true)]
        [InlineData("image/png", true)]
        [InlineData("video/mp4", true)]
        [InlineData("application/octet-stream", false)]
        [InlineData("", false)]
        public void ContentTypeValidation_Works(string ct, bool expected)
        {
            Assert.Equal(expected, FileValidationService.IsContentTypeAllowed(ct));
        }

        [Fact]
        public void SizeValidation_Works()
        {
            // 10 MB limit
            Assert.True(FileValidationService.IsUnderMaxSize(5 * 1024 * 1024, 10));
            Assert.False(FileValidationService.IsUnderMaxSize(11 * 1024 * 1024, 10));
        }
    }
}
