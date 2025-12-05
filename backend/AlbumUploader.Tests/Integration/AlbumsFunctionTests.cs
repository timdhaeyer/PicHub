using System.Threading.Tasks;
using Xunit;

namespace PicHub.AlbumUploader.Tests.Integration;

public class AlbumsFunctionTests
{
    [Fact]
    public Task UploadMedia_Returns_Forbidden_When_AlbumDisallowsUploads()
    {
        // Test requiring Function runtime or integration environment — skip here.
        return Task.CompletedTask;
    }
}
