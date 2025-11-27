using PicHub.AlbumUploader.Models;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

using MediatR;

public class CreateAlbumHandler(IAlbumRepository repo) : IRequestHandler<CreateAlbumCommand, CreateAlbumResult>
{
    private readonly IAlbumRepository _repo = repo;

    public Task<CreateAlbumResult> Handle(CreateAlbumCommand command, CancellationToken cancellationToken)
    {
        var album = new Album
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Description = command.Description,
            PublicToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "-").Replace("/", "_")
        };
        _repo.InsertAlbum(album);
        return Task.FromResult(new CreateAlbumResult(album.PublicToken));
    }
}
