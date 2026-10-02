using Ardalis.Result;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Features.Payments.Commands.HandlePaymentSucceeded;

sealed class HandlePaymentSucceededCommandHandler(
    IServiceProvider serviceProvider,
    ILogger<HandlePaymentSucceededCommandHandler> logger
) : IRequestHandler<HandlePaymentSucceededCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        HandlePaymentSucceededCommand command,
        CancellationToken cancellationToken)
    {
        var statusHandler = serviceProvider.GetKeyedService<IFinancialTargetStatusHandler>(command.Purpose);

        if (statusHandler is null)
        {
            logger.LogError(
                "Financial target status handler was not found for succeeded payment {PaymentId} for {Purpose}", 
                command.PaymentId, 
                command.Purpose);
            return Result.Error("Financial target status handler was not found for succeeded payment");
        }

        var statusHandle = await statusHandler.HandleSucceededAsync(new SucceededStatusRequest(
            command.ReferenceId,
            command.PaymentId
        ), cancellationToken);

        if (!statusHandle.IsSuccess)
        {
            logger.LogError(
                "Failed to handle status for succeeded payment {PaymentId} for {Purpose}", 
                command.PaymentId, 
                command.Purpose);
            return Result.Error("Failed to handle status for succeeded payment");
        }

        return new IdResponse(command.PaymentId);
    }
}
