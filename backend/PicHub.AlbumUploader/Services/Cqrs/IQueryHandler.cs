using MediatR;

namespace PicHub.AlbumUploader.Services.Cqrs;

public interface IQueryHandler<TQuery, TResult> where TQuery : IRequest<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}
