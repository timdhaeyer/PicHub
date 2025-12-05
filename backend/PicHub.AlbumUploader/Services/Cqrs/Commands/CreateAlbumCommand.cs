using MediatR;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public record CreateAlbumCommand(string Title, string? Description) : IRequest<CreateAlbumResult>;

public record CreateAlbumResult(string PublicToken);
