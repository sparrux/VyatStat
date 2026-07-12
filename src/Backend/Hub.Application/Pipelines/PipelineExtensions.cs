using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Pipelines;

static class PipelineExtensions
{
    public static void AddDecoratedHandler<TRequest, TResponse, THandler>(
        this IServiceCollection services)
        where TRequest : notnull
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        services.AddScoped<THandler>();

        services.AddScoped<IRequestHandler<TRequest, TResponse>>(provider =>
        {
            IRequestHandler<TRequest, TResponse> handler = provider.GetRequiredService<THandler>();

            handler = new LoggingHandlerDecorator<TRequest, TResponse>(
                handler,
                provider.GetRequiredService<ILogger<IRequestHandler<TRequest, TResponse>>>()
            );

            handler = new ValidationHandlerDecorator<TRequest, TResponse>(
                handler,
                provider.GetRequiredService<IEnumerable<IValidator<TRequest>>>()
            );

            return handler;
        });
    }
}