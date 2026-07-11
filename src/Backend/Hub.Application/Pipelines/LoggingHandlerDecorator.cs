using Ardalis.Result;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Pipelines;

sealed class LoggingHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    ILogger<IRequestHandler<TRequest, TResponse>> logger
) : IRequestHandler<TRequest, TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        TResponse response;
        logger.LogInformation("Handling request: {Request}", request);

        try
        {
            response = await inner.Handle(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling request: {Request}", request);
            throw;
        }
        finally
        {
            logger.LogInformation("Handled request: {Request}", request);
        }

        return response;
    }
}