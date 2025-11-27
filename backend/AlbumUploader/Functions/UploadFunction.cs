using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using AlbumUploader.Services;
using AlbumUploader.Models;
using System;
using System.IO;

namespace AlbumUploader.Functions
{
    public static class UploadFunction
    {
        [Function("UploadMedia")]
        public static async Task<HttpResponseData> UploadMedia([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "albums/{publicToken}")] HttpRequestData req, string publicToken)
        {
            var env = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING") ?? Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var sqlConn = Environment.GetEnvironmentVariable("SqlConnectionString");

            var resp = req.CreateResponse();

            if (string.IsNullOrEmpty(env) || string.IsNullOrEmpty(sqlConn))
            {
                resp.StatusCode = HttpStatusCode.InternalServerError;
                await resp.WriteStringAsync("Missing storage or sql connection string in environment.");
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

            var quota = new QuotaService();
            var validator = new FileValidationService();

            if (!req.Headers.TryGetValues("Content-Type", out var ctVals))
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                await resp.WriteStringAsync("Content-Type header missing");
                return resp;
            }

            var contentType = ctVals.FirstOrDefault() ?? string.Empty;
            // Expect multipart/form-data; boundary=...
            var mediaType = new Microsoft.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            if (!Microsoft.Net.Http.Headers.MediaTypeHeaderValue.TryParse(contentType, out var parsed) ||
                !parsed.MediaType.Equals("multipart/form-data", StringComparison.OrdinalIgnoreCase))
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                await resp.WriteStringAsync("Content-Type must be multipart/form-data");
                return resp;
            }

            var boundary = Microsoft.AspNetCore.WebUtilities.MultipartRequestHelper.GetBoundary(parsed, 70);
            var reader = new Microsoft.AspNetCore.WebUtilities.MultipartReader(boundary, req.Body);
            var section = await reader.ReadNextSectionAsync();
            bool uploadedAny = false;

            var blobService = new BlobServiceClient(env);
            var container = blobService.GetBlobContainerClient("pichub-local");
            await container.CreateIfNotExistsAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = Microsoft.Net.Http.Headers.ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") && !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    var fileName = contentDisposition.FileName.Value.Trim('"');
                    // Stream to temp file first to evaluate size
                    var tmp = Path.GetTempFileName();
                    long bytesCopied = 0;
                    using (var target = File.Create(tmp))
                    {
                        await section.Body.CopyToAsync(target);
                        bytesCopied = target.Length;
                    }

                    // Determine per-file max: album setting or environment default (MB)
                    var defaultMaxMb = 50; // default fallback
                    var envMax = Environment.GetEnvironmentVariable("DEFAULT_MAX_FILE_SIZE_MB");
                    if (int.TryParse(envMax, out var parsedMax)) defaultMaxMb = parsedMax;
                    var albumMax = album.MaxFileSizeMb ?? defaultMaxMb;

                    if (!validator.IsUnderMaxSize(bytesCopied, albumMax))
                    {
                        resp.StatusCode = HttpStatusCode.RequestEntityTooLarge;
                        await resp.WriteStringAsync($"File exceeds per-file limit of {albumMax} MB");
                        return resp;
                    }

                    if (!quota.CanUpload(album, bytesCopied))
                    {
                        resp.StatusCode = HttpStatusCode.RequestEntityTooLarge;
                        await resp.WriteStringAsync("Album quota exceeded");
                        return resp;
                    }

                    // Validate content-type if present
                    var sectionContentType = section.ContentType;
                    if (!string.IsNullOrEmpty(sectionContentType) && !validator.IsContentTypeAllowed(sectionContentType))
                    {
                        resp.StatusCode = HttpStatusCode.UnsupportedMediaType;
                        await resp.WriteStringAsync("Content type not allowed");
                        return resp;
                    }

                    var blobName = $"{album.Id}/{Guid.NewGuid()}_{fileName}";
                    var blob = container.GetBlobClient(blobName);
                    using (var fs = File.OpenRead(tmp))
                    {
                        await blob.UploadAsync(fs, overwrite: true);
                    }

                    var mediaItem = new MediaItem
                    {
                        Id = Guid.NewGuid(),
                        AlbumId = album.Id,
                        Filename = fileName,
                        SizeBytes = bytesCopied,
                        StoragePath = blobName,
                        UploadedAt = DateTime.UtcNow,
                        IsProcessed = false
                    };

                    repo.InsertMediaItem(mediaItem);
                    uploadedAny = true;
                    File.Delete(tmp);
                }

                section = await reader.ReadNextSectionAsync();
            }

            if (!uploadedAny)
            {
                resp.StatusCode = HttpStatusCode.BadRequest;
                await resp.WriteStringAsync("No files found in multipart form data");
                return resp;
            }

            resp.StatusCode = HttpStatusCode.Created;
            await resp.WriteStringAsync("{ \"message\": \"Uploaded\" }");
            return resp;
        }
    }
}
