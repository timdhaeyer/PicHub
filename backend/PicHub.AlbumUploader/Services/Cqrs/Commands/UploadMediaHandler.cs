using MediatR;

namespace PicHub.AlbumUploader.Services.Cqrs.Commands;

public class UploadMediaHandler(IAlbumRepository repo) : IRequestHandler<UploadMediaCommand, Unit>
{
    private readonly IAlbumRepository _repo = repo;

    public Task<Unit> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        _repo.InsertMediaItem(request.Item);
        return Task.FromResult(Unit.Value);
    }
}
