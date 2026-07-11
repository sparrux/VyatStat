using Ardalis.Result;

namespace Hub.Application.Pipelines;

public interface IRequestHandler<in TRequest, TResponse>
{
    Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}