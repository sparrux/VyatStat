using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Commands.ConfirmCash;

sealed class ConfirmCashCommandHandler(
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<ConfirmCashCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(
        ConfirmCashCommand command,
        CancellationToken cancellationToken)
    {
        var loaded = await CashPaymentAccess.LoadAsync(
            paymentsDbContext,
            command.PaymentId,
            cancellationToken);
        if (!loaded.IsSuccess)
            return loaded.Map();

        var (payment, attempt) = loaded.Value;

        if (payment.Status is PaymentStatus.Succeeded)
            return Result.Success(PaymentResponse.From(payment, approvalUrl: null));

        if (payment.Status is PaymentStatus.Cancelled)
            return Result.Error("Cancelled payment cannot be confirmed");

        if (attempt.ProviderPaymentId is null)
            return Result.Error("Cash payment has not been started");

        var applied = PaymentCheckout.ApplyProviderResult(
            payment,
            attempt.Id,
            attempt.ProviderPaymentId,
            PaymentAttemptStatus.Succeeded);
        
        await paymentsDbContext.SaveChangesAsync(cancellationToken);

        if (!applied.IsSuccess)
            return applied.Map();

        return Result.Success(PaymentResponse.From(payment, approvalUrl: null));
    }
}
