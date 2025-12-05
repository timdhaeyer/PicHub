using MediatR;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Queries;

public record GetAlbumQuery(string PublicToken) : IRequest<Album?>;
