using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Features.Payments.Services.FinancialTargets;

sealed class DonationStatusHandler(
    IPaymentsDbContext paymentsDbContext,
    ILogger<DonationStatusHandler> logger
) : IFinancialTargetStatusHandler
{
    public async Task<Result> HandleSucceededAsync(
        SucceededStatusRequest request, 
        CancellationToken cancellationToken)
    {
        var donation = await paymentsDbContext.Donations
            .FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);

        if (donation is null)
        {
            logger.LogWarning(
                "Donation {DonationId} was not found for succeeded payment {PaymentId}",
                request.TargetId,
                request.PaymentId);
            return Result.Success();
        }

        var completed = donation.Complete();
    
        if (!completed.IsSuccess)
        {
            logger.LogWarning(
                "Donation {DonationId} was not completed after payment {PaymentId}: {Error}",
                donation.Id,
                request.PaymentId,
                completed.Errors.FirstOrDefault());
            return Result.Success();
        }

        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public Task<Result> HandleFailedAsync(
        FailedStatusRequest request, 
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());
    }

    public async Task<Result> HandleCancelledAsync(
        CancelledStatusRequest request, 
        CancellationToken cancellationToken)
    {
        var donation = await paymentsDbContext.Donations
            .FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);

        if (donation is null)
        {
            logger.LogWarning(
                "Donation {DonationId} was not found for cancelled payment {PaymentId}",
                request.TargetId,
                request.PaymentId);
            return Result.Success();
        }

        var cancelled = donation.Cancel();
    
        if (!cancelled.IsSuccess)
        {
            logger.LogWarning(
                "Donation {DonationId} was not cancelled after payment {PaymentId}: {Error}",
                donation.Id,
                request.PaymentId,
                cancelled.Errors.FirstOrDefault());
            return Result.Success();
        }

        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}