using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Commands.CancelCash;

sealed class CancelCashCommandHandler(
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<CancelCashCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(
        CancelCashCommand command,
        CancellationToken cancellationToken)
    {
        var loaded = await CashPaymentAccess.LoadAsync(
            paymentsDbContext,
            command.PaymentId,
            cancellationToken);
        if (!loaded.IsSuccess)
            return loaded.Map();

        var (payment, attempt) = loaded.Value;

        if (payment.Status is PaymentStatus.Cancelled)
            return Result.Success(PaymentResponse.From(payment, approvalUrl: null));

        if (payment.Status is PaymentStatus.Succeeded)
            return Result.Error("Succeeded cash donation cannot be cancelled");

        var applied = PaymentCheckout.ApplyGatewayStatus(
            payment,
            attempt.Id,
            PaymentAttemptStatus.Cancelled);

        if (!applied.IsSuccess)
        {
            await paymentsDbContext.SaveChangesAsync(cancellationToken);
            return applied.Map();
        }

        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(PaymentResponse.From(payment, approvalUrl: null));
    }
}
