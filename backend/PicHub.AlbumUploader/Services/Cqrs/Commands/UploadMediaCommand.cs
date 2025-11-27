using MediatR;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public record UploadMediaCommand(MediaItem Item) : IRequest<Unit>;
