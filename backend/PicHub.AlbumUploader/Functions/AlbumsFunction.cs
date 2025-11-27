using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PicHub.AlbumUploader.Services;

namespace PicHub.AlbumUploader.Functions;

public class AlbumsFunction(IAlbumRepository repo)
{
    private readonly IAlbumRepository _repo = repo;

    [Function("GetAlbum")]
    public async Task<HttpResponseData> GetAlbum([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "albums/{publicToken}")] HttpRequestData req, string publicToken)
    {
        var resp = req.CreateResponse();

        var album = _repo.GetByPublicToken(publicToken);
        if (album == null)
        {
            resp.StatusCode = HttpStatusCode.NotFound;
            await resp.WriteStringAsync("Album not found");
            return resp;
        }

        resp.StatusCode = HttpStatusCode.OK;
        resp.Headers.Add("Content-Type", "application/json");
        await resp.WriteStringAsync($"{{ \"id\": \"{album.Id}\", \"title\": \"{album.Title}\" }}");
        return resp;
    }
}
