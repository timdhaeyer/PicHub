using MediatR;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Queries;

public record GetPublicAlbumQuery(string PublicToken) : IRequest<PublicAlbumDto?>;
