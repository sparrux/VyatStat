using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Features.Payments.Commands.HandlePaymentFailed;

sealed class HandlePaymentFailedCommandHandler(
    IPaymentsDbContext paymentsDbContext,
    ILogger<HandlePaymentFailedCommandHandler> logger
) : IRequestHandler<HandlePaymentFailedCommand, IdResponse>
{
    public Task<Result<IdResponse>> Handle(
        HandlePaymentFailedCommand command,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success(new IdResponse(command.PaymentId)));
    }
}
