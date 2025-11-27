using MediatR;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public record ProcessMediaUploadCommand(
    Album Album,
    Stream RequestBody,
    string ContentType
) : IRequest<ProcessMediaUploadResult>;

public record ProcessMediaUploadResult(
    bool Success,
    string? ErrorMessage = null,
    System.Net.HttpStatusCode? ErrorStatusCode = null,
    List<UploadedMediaInfo>? UploadedItems = null
);

public record UploadedMediaInfo(
    Guid Id,
    string Filename,
    long Size,
    string StoragePath
);
