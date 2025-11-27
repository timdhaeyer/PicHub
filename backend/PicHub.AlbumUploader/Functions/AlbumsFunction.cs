using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using AlbumUploader.Services;
using System.Threading.Tasks;
using System;

namespace PicHub.AlbumUploader.Functions
{
    public static class AlbumsFunction
    {
        [Function("GetAlbum")]
        public static async Task<HttpResponseData> GetAlbum([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "albums/{publicToken}")] HttpRequestData req, string publicToken)
        {
            var sqlConn = Environment.GetEnvironmentVariable("SqlConnectionString");
            var resp = req.CreateResponse();

            if (string.IsNullOrEmpty(sqlConn))
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                await resp.WriteStringAsync("SqlConnectionString not configured");
                return resp;
            }

            var repo = new AlbumRepository(sqlConn);
            var album = repo.GetByPublicToken(publicToken);
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
}
