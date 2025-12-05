using MediatR;
using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Queries;

public class GetPublicAlbumHandler : IRequestHandler<GetPublicAlbumQuery, PublicAlbumDto?>
{
    private readonly IAlbumRepository _repo;
    public GetPublicAlbumHandler(IAlbumRepository repo) => _repo = repo;

    public Task<PublicAlbumDto?> Handle(GetPublicAlbumQuery query, CancellationToken cancellationToken)
    {
        var album = _repo.GetByPublicToken(query.PublicToken);
        if (album == null) return Task.FromResult<PublicAlbumDto?>(null);

        var items = _repo.GetMediaItems(album.Id);

        var dto = new PublicAlbumDto
        {
            Id = album.Id,
            Title = album.Title,
            Description = album.Description,
            AllowUploads = album.AllowUploads,
            MaxFileSizeMb = album.MaxFileSizeMb,
            AlbumSizeTshirt = album.AlbumSizeTshirt,
            Items = items
        };

        return Task.FromResult<PublicAlbumDto?>(dto);
    }
}
