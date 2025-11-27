using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using PicHub.AlbumUploader.Services;
using PicHub.AlbumUploader.Models;
using MediatR;
using PicHub.AlbumUploader.Services.Cqrs.Commands;
using PicHub.AlbumUploader.Services.Cqrs.Queries;

namespace PicHub.AlbumUploader.Functions;

/// <summary>
/// Azure Functions for album and media management operations.
/// </summary>
public class AlbumsFunction
{
    private readonly IMediator _mediator;
    private readonly AdminAuthService _auth;

    public AlbumsFunction(IMediator mediator, AdminAuthService auth)
    {
        _mediator = mediator;
        _auth = auth;
    }

    /// <summary>
    /// GET /api/albums/{publicToken} - Retrieves album information by public token.
    /// </summary>
    [Function("GetAlbum")]
    public async Task<HttpResponseData> GetAlbum(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/albums/{publicToken}")] 
        HttpRequestData req, 
        string publicToken)
    {
        var album = await _mediator.Send(new GetAlbumQuery(publicToken));
        
        if (album == null)
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.NotFound, "Album not found");
        }

        return await CreateJsonResponseAsync(req, HttpStatusCode.OK, new 
        { 
            id = album.Id, 
            title = album.Title 
        });
    }

    /// <summary>
    /// POST /api/admin/albums - Creates a new album (admin only).
    /// </summary>
    [Function("CreateAlbum")]
    public async Task<HttpResponseData> CreateAlbum(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/admin/albums")] 
        HttpRequestData req)
    {
        // Authorization check
        if (!_auth.IsAuthorized(req))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.Unauthorized, 
                "Unauthorized: missing or invalid X-Admin-Auth header");
        }

        // Request validation
        var body = await req.ReadFromJsonAsync<CreateAlbumRequest>();
        if (body == null || string.IsNullOrWhiteSpace(body.Title))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, 
                "Missing title in request body");
        }

        // Create album via CQRS
        var cmd = new CreateAlbumCommand(body.Title, body.Description);
        var result = await _mediator.Send(cmd);
        var token = result.PublicToken ?? string.Empty;
        
        var publicUrl = $"/api/albums/{token}";
        var response = await CreateJsonResponseAsync(req, HttpStatusCode.Created, new 
        { 
            publicToken = token, 
            publicUrl 
        });
        
        response.Headers.Add("Location", publicUrl);
        return response;
    }

    /// <summary>
    /// POST /api/albums/{publicToken}/media - Uploads media files to an album.
    /// </summary>
    [Function("UploadMedia")]
    public async Task<HttpResponseData> UploadMedia(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/albums/{publicToken}/media")] 
        HttpRequestData req, 
        string publicToken)
    {
        // Get album via CQRS query
        var album = await _mediator.Send(new GetAlbumQuery(publicToken));
        if (album == null)
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.NotFound, "Album not found");
        }

        if (!album.AllowUploads)
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.Forbidden, 
                "Uploads are not allowed for this album");
        }

        // Validate Content-Type header
        var contentType = GetContentTypeHeader(req);
        if (contentType == null)
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, 
                "Content-Type header missing");
        }

        if (!contentType.Contains("multipart/form-data", StringComparison.OrdinalIgnoreCase))
        {
            return await CreateErrorResponseAsync(req, HttpStatusCode.BadRequest, 
                "Content-Type must be multipart/form-data");
        }

        // Delegate upload logic to command handler
        var command = new ProcessMediaUploadCommand(album, req.Body, contentType);
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return await CreateErrorResponseAsync(req, 
                result.ErrorStatusCode ?? HttpStatusCode.BadRequest, 
                result.ErrorMessage ?? "Upload failed");
        }

        // Build success response
        var response = await CreateJsonResponseAsync(req, HttpStatusCode.Created, new 
        { 
            message = "Uploaded", 
            items = result.UploadedItems?.Select(i => new 
            { 
                id = i.Id, 
                filename = i.Filename, 
                size = i.Size, 
                storagePath = i.StoragePath 
            })
        });

        // Set Location header to first uploaded media item
        if (result.UploadedItems?.Count > 0)
        {
            var location = $"/api/albums/{publicToken}/media/{result.UploadedItems[0].Id}";
            response.Headers.Add("Location", location);
        }

        return response;
    }

    #region Helper Methods

    /// <summary>
    /// Creates an error response with the specified status code and message.
    /// </summary>
    private static async Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData req, 
        HttpStatusCode statusCode, 
        string message)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteStringAsync(message);
        return response;
    }

    /// <summary>
    /// Creates a JSON response with the specified status code and data.
    /// </summary>
    private static async Task<HttpResponseData> CreateJsonResponseAsync<T>(
        HttpRequestData req, 
        HttpStatusCode statusCode, 
        T data)
    {
        var response = req.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(data);
        return response;
    }

    /// <summary>
    /// Extracts the Content-Type header value from the request.
    /// </summary>
    private static string? GetContentTypeHeader(HttpRequestData req)
    {
        return req.Headers.TryGetValues("Content-Type", out var values) 
            ? values.FirstOrDefault() 
            : null;
    }

    #endregion
}
