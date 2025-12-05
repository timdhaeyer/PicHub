using MediatR;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public record CreateAlbumCommand(
    string Title,
    string? Description,
    bool AllowUploads,
    int? MaxFileSizeMb,
    string? AlbumSizeTshirt,
    int? RetentionDays
) : IRequest<CreateAlbumResult>;

public record CreateAlbumResult(string PublicToken);
