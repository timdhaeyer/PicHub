using System.Net;
using Azure.Storage.Blobs;
using MediatR;
using PicHub.AlbumUploader.Models;
using Microsoft.Net.Http.Headers;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public class ProcessMediaUploadHandler : IRequestHandler<ProcessMediaUploadCommand, ProcessMediaUploadResult>
{
    private readonly IAlbumRepository _repo;
    private readonly BlobServiceClient _blobService;

    public ProcessMediaUploadHandler(IAlbumRepository repo, BlobServiceClient blobService)
    {
        _repo = repo;
        _blobService = blobService;
    }

    public async Task<ProcessMediaUploadResult> Handle(ProcessMediaUploadCommand request, CancellationToken cancellationToken)
    {
        var album = request.Album;

        // Parse content type and get boundary
        if (!MediaTypeHeaderValue.TryParse(request.ContentType, out var parsed))
        {
            return new ProcessMediaUploadResult(
                false,
                "Invalid Content-Type header",
                HttpStatusCode.BadRequest
            );
        }

        var boundary = GetBoundary(parsed, 70);
        var reader = new Microsoft.AspNetCore.WebUtilities.MultipartReader(boundary, request.RequestBody);
        var uploadedItems = new List<UploadedMediaInfo>();
        var tempFiles = new List<string>();

        var container = _blobService.GetBlobContainerClient(
            Environment.GetEnvironmentVariable("BLOB_CONTAINER") ?? "pichub-local"
        );
        await container.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

        try
        {
            var section = await reader.ReadNextSectionAsync(cancellationToken);

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition,
                    out var contentDisposition
                );

                if (hasContentDispositionHeader &&
                    contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    var fileName = contentDisposition.FileName.Value.Trim('"');
                    var tmp = Path.GetTempFileName();
                    tempFiles.Add(tmp);

                    long bytesCopied;
                    using (var target = File.Create(tmp))
                    {
                        await section.Body.CopyToAsync(target, cancellationToken);
                        bytesCopied = target.Length;
                    }

                    // Validate file size
                    var defaultMaxMb = 50;
                    var envMax = Environment.GetEnvironmentVariable("DEFAULT_MAX_FILE_SIZE_MB");
                    if (int.TryParse(envMax, out var parsedMax)) defaultMaxMb = parsedMax;
                    var albumMax = album.MaxFileSizeMb ?? defaultMaxMb;

                    if (!FileValidationService.IsUnderMaxSize(bytesCopied, albumMax))
                    {
                        return new ProcessMediaUploadResult(
                            false,
                            $"File exceeds per-file limit of {albumMax} MB",
                            HttpStatusCode.RequestEntityTooLarge
                        );
                    }

                    // Validate quota
                    if (!QuotaService.CanUpload(album, bytesCopied))
                    {
                        return new ProcessMediaUploadResult(
                            false,
                            "Album quota exceeded",
                            HttpStatusCode.RequestEntityTooLarge
                        );
                    }

                    // Validate content type
                    var sectionContentType = section.ContentType;
                    if (!string.IsNullOrEmpty(sectionContentType) &&
                        !FileValidationService.IsContentTypeAllowed(sectionContentType))
                    {
                        return new ProcessMediaUploadResult(
                            false,
                            "Content type not allowed",
                            HttpStatusCode.UnsupportedMediaType
                        );
                    }

                    // Upload to blob storage
                    var blobName = $"{album.Id}/{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
                    var blob = container.GetBlobClient(blobName);
                    using (var fs = File.OpenRead(tmp))
                    {
                        await blob.UploadAsync(fs, overwrite: true, cancellationToken: cancellationToken);
                    }

                    // Create media item
                    var mediaItem = new MediaItem
                    {
                        Id = Guid.NewGuid(),
                        AlbumId = album.Id,
                        Filename = fileName,
                        SizeBytes = bytesCopied,
                        ContentType = sectionContentType,
                        StoragePath = blobName,
                        UploadedAt = DateTime.UtcNow,
                        IsProcessed = false
                    };

                    // Insert into repository
                    _repo.InsertMediaItem(mediaItem);

                    uploadedItems.Add(new UploadedMediaInfo(
                        mediaItem.Id,
                        mediaItem.Filename,
                        mediaItem.SizeBytes,
                        mediaItem.StoragePath
                    ));
                }

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            if (uploadedItems.Count == 0)
            {
                return new ProcessMediaUploadResult(
                    false,
                    "No files found in multipart form data",
                    HttpStatusCode.BadRequest
                );
            }

            return new ProcessMediaUploadResult(
                true,
                UploadedItems: uploadedItems
            );
        }
        finally
        {
            // Clean up temp files
            foreach (var f in tempFiles)
            {
                try { File.Delete(f); } catch { /* Ignore cleanup errors */ }
            }
        }
    }

    private static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = contentType.Boundary.Value;
        if (string.IsNullOrWhiteSpace(boundary))
        {
            throw new InvalidDataException("Missing content-type boundary.");
        }
        if (boundary.Length > lengthLimit)
        {
            throw new InvalidDataException($"Multipart boundary length limit {lengthLimit} exceeded.");
        }
        return boundary;
    }
}
