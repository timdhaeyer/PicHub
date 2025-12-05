using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PicHub.AlbumUploader.Services.Storage;
using PicHub.AlbumUploader.Services;
using System.Net;

namespace PicHub.AlbumUploader.Functions;

public class BlobProxyFunction
{
    private readonly IBlobService _blob;
    private readonly IAlbumRepository _repo;

    public BlobProxyFunction(IBlobService blob, IAlbumRepository repo)
    {
        _blob = blob;
        _repo = repo;
    }

    [Function("GetBlob")]
    public async Task<HttpResponseData> GetBlob(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "blobs/{container}/{*blobPath}")]
        HttpRequestData req,
        string container,
        string blobPath)
    {
        try
        {
            Console.WriteLine($"BlobProxy requested: container={container}, blobPath={blobPath}");
            var stream = await _blob.OpenReadAsync(container, blobPath);

            // Try to obtain content type from repository metadata
            var media = _repo.GetMediaItemByStoragePath(blobPath);
            var contentType = media?.ContentType ?? "application/octet-stream";

            Console.WriteLine($"Serving blob: contentType={contentType}");
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", contentType);
            await stream.CopyToAsync(response.Body);
            return response;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Blob not found, attempting redirect fallback...");
            // Try to get a direct URI and redirect (useful for Azure storage URLs)
            try
            {
                var uri = await _blob.GetBlobUriAsync(container, blobPath);
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                {
                    var redirect = req.CreateResponse(HttpStatusCode.Redirect);
                    redirect.Headers.Add("Location", uri.ToString());
                    return redirect;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redirect fallback failed: {ex.Message}");
            }

            return req.CreateResponse(HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Blob proxy error: {ex.Message}");
            return req.CreateResponse(HttpStatusCode.InternalServerError);
        }
    }
}
