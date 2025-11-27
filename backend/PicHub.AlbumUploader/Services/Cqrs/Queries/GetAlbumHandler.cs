using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Queries;

using MediatR;

public class GetAlbumHandler : IRequestHandler<GetAlbumQuery, Album?>
{
    private readonly IAlbumRepository _repo;
    public GetAlbumHandler(IAlbumRepository repo) => _repo = repo;

    public Task<Album?> Handle(GetAlbumQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(_repo.GetByPublicToken(query.PublicToken));
    }
}
