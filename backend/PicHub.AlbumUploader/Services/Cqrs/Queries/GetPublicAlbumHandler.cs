using MediatR;
using PicHub.AlbumUploader.Models;
using Microsoft.Extensions.Logging;

namespace PicHub.AlbumUploader.Services.Cqrs.Queries;

public class GetPublicAlbumHandler : IRequestHandler<GetPublicAlbumQuery, PublicAlbumDto?>
{
    private readonly IAlbumRepository _repo;
    private readonly Services.Storage.IBlobService _blobService;
    private readonly Microsoft.Extensions.Logging.ILogger<GetPublicAlbumHandler> _logger;

    public GetPublicAlbumHandler(IAlbumRepository repo, Services.Storage.IBlobService blobService, Microsoft.Extensions.Logging.ILogger<GetPublicAlbumHandler> logger)
    {
        _repo = repo;
        _blobService = blobService;
        _logger = logger;
    }

    public async Task<PublicAlbumDto?> Handle(GetPublicAlbumQuery query, CancellationToken cancellationToken)
    {
        var album = _repo.GetByPublicToken(query.PublicToken);
        if (album == null) return null;

        var items = _repo.GetMediaItems(album.Id);

        var itemDtos = new List<Models.MediaItemDto>();
        var containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER") ?? "pichub-local";

        foreach (var i in items)
        {
            Uri? uri = null;
            try
            {
                // Prefer a same-origin proxy URL so browsers can fetch without CORS issues
                // Safely encode path segments but preserve slashes
                var rawPath = (i.StoragePath ?? string.Empty).TrimStart('/');
                var encoded = Uri.EscapeDataString(rawPath).Replace("%2F", "/");
                var proxy = $"/api/blobs/{containerName}/{encoded}";
                _logger.LogDebug("GetPublicAlbumHandler: proxy={Proxy}", proxy);
                uri = new Uri(proxy, UriKind.Relative);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to construct proxy URI for item {ItemId}", i.Id);
                uri = null;
            }

            itemDtos.Add(new Models.MediaItemDto
            {
                Id = i.Id,
                Filename = i.Filename,
                SizeBytes = i.SizeBytes,
                ContentType = i.ContentType,
                StoragePath = i.StoragePath,
                UploadedAt = i.UploadedAt,
                BlobUri = uri
            });
        }

        var dto = new PublicAlbumDto
        {
            Id = album.Id,
            Title = album.Title,
            Description = album.Description,
            AllowUploads = album.AllowUploads,
            MaxFileSizeMb = album.MaxFileSizeMb,
            AlbumSizeTshirt = album.AlbumSizeTshirt,
            Items = itemDtos
        };

        return dto;
    }
}
