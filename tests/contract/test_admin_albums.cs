using System.Net.Http;
using Xunit;

namespace PicHub.AlbumUploader.ContractTests;

public class AdminAlbumsContractTests
{
    [Fact]
    public async Task AdminEndpoints_RequireAuthentication()
    {
        // Placeholder: implement HTTP client and test that admin endpoints return 401/403 for anonymous
        await Task.CompletedTask;
    }
}
