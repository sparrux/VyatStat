using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;

namespace Hub.Application.Pipelines;

sealed class ValidationHandlerDecorator<TRequest, TResponse>(
    IRequestHandler<TRequest, TResponse> inner,
    IEnumerable<IValidator<TRequest>> validators
) : IRequestHandler<TRequest, TResponse>
{
    public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = await Task.WhenAll(
            validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

        var errors = failures
            .SelectMany(x => x.AsErrors())
            .Where(x => x != null)
            .ToList();
        
        return errors.Count > 0
            ? Result.Invalid(errors)
            : await inner.Handle(request, cancellationToken);
    }
}