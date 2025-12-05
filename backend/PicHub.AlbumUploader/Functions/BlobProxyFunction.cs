using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PicHub.AlbumUploader.Services.Storage;
using PicHub.AlbumUploader.Services;
using Microsoft.Extensions.Logging;
using System.Net;

namespace PicHub.AlbumUploader.Functions;

public class BlobProxyFunction
{
    private readonly IBlobService _blob;
    private readonly IAlbumRepository _repo;
    private readonly Microsoft.Extensions.Logging.ILogger<BlobProxyFunction> _logger;

    public BlobProxyFunction(IBlobService blob, IAlbumRepository repo, Microsoft.Extensions.Logging.ILogger<BlobProxyFunction> logger)
    {
        _blob = blob;
        _repo = repo;
        _logger = logger;
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
            _logger.LogDebug("BlobProxy requested: container={Container}, blobPath={BlobPath}", container, blobPath);

            // Ensure the blob belongs to a publicly-visible media item (prevent arbitrary blob access)
            var media = _repo.GetMediaItemByStoragePath(blobPath);
            if (media == null)
            {
                _logger.LogWarning("Blob requested but no media item found for storage path {BlobPath}", blobPath);
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var album = _repo.GetById(media.AlbumId);
            // Only allow proxy access for albums that have a public token (public albums)
            if (album == null || string.IsNullOrEmpty(album.PublicToken))
            {
                _logger.LogWarning("Media item {MediaId} belongs to a non-public album; denying proxy access.", media.Id);
                return req.CreateResponse(HttpStatusCode.Forbidden);
            }

            var stream = await _blob.OpenReadAsync(container, blobPath);
            var contentType = media?.ContentType ?? "application/octet-stream";

            _logger.LogDebug("Serving blob: contentType={ContentType}", contentType);
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
