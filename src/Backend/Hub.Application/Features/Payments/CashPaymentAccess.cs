using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments;

static class CashPaymentAccess
{
    public static async Task<Result<(Payment Payment, PaymentAttempt Attempt)>> LoadAsync(
        IPaymentsDbContext paymentsDbContext,
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        var payment = await paymentsDbContext.Payments
            .Include(x => x.Attempts)
            .FirstOrDefaultAsync(x => x.Id == paymentId, cancellationToken);

        if (payment is null)
            return Result.Error("Payment not found for donation");

        var attempt = payment.Attempts
            .OrderByDescending(x => x.AttemptNumber)
            .FirstOrDefault();

        if (attempt is null)
            return Result.Error("Payment has not been started");

        if (!PaymentCheckout.IsCash(attempt))
            return Result.Error("Only cash payments can be managed this way");

        return Result.Success((payment, attempt));
    }
}
