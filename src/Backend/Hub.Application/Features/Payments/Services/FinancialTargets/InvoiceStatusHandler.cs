using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hub.Application.Features.Payments.Services.FinancialTargets;

sealed class InvoiceStatusHandler(
    IServiceProvider serviceProvider,
    IPaymentsDbContext paymentsDbContext,
    ILogger<InvoiceStatusHandler> logger
) : IFinancialTargetStatusHandler
{
    public async Task<Result> HandleSucceededAsync(
        SucceededStatusRequest request, 
        CancellationToken cancellationToken)
    {
        var invoice = await paymentsDbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);

        if (invoice is null)
        {
            logger.LogWarning(
                "Invoice {InvoiceId} was not found for succeeded payment {PaymentId}",
                request.TargetId,
                request.PaymentId);
            return Result.Success();
        }

        var completed = invoice.MarkAsPaid();
    
        if (!completed.IsSuccess)
        {
            logger.LogWarning(
                "Invoice {InvoiceId} was not completed after payment {PaymentId}: {Error}",
                invoice.Id,
                request.PaymentId,
                completed.Errors.FirstOrDefault());
            return Result.Success();
        }

        var sourceTypeHandler = serviceProvider.GetKeyedService<IFinancialTargetStatusHandler>(
            invoice.SourceType.ToString());

        if (sourceTypeHandler is not null && invoice.SourceId is not null && invoice.PaymentId is not null)
        {
            var sourceHandle = await sourceTypeHandler.HandleSucceededAsync(new SucceededStatusRequest(
                invoice.SourceId.Value, 
                invoice.PaymentId.Value
            ), cancellationToken);
            
            if (!sourceHandle.IsSuccess)
                logger.LogError(
                    "Source {SourceType} {SourceId} was not completed after payment {PaymentId}: Error: {Error}",
                    invoice.SourceType,
                    invoice.SourceId,
                    invoice.PaymentId,
                    sourceHandle.Errors.FirstOrDefault());
            else 
                logger.LogInformation(
                    "Source {SourceType} {SourceId} was completed after payment {PaymentId}",
                    invoice.SourceType,
                    invoice.SourceId,
                    invoice.PaymentId);
        }
        else
        {
            logger.LogInformation(
                "Invoice {InvoiceId} has no source {SourceType} {SourceId} entity for handling, PaymentId: {PaymentId}",
                invoice.Id,
                invoice.SourceType,
                invoice.SourceId,
                request.PaymentId);
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
        var invoice = await paymentsDbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);

        if (invoice is null)
        {
            logger.LogWarning(
                "Invoice {InvoiceId} was not found for cancelled payment {PaymentId}",
                request.TargetId,
                request.PaymentId);
            return Result.Success();
        }

        var completed = invoice.Void();
    
        if (!completed.IsSuccess)
        {
            logger.LogWarning(
                "Invoice {InvoiceId} was not void after payment {PaymentId}: {Error}",
                invoice.Id,
                request.PaymentId,
                completed.Errors.FirstOrDefault());
            return Result.Success();
        }

        var sourceTypeHandler = serviceProvider.GetKeyedService<IFinancialTargetStatusHandler>(
            invoice.SourceType.ToString());

        if (sourceTypeHandler is not null && invoice.SourceId is not null && invoice.PaymentId is not null)
        {
            var sourceHandle = await sourceTypeHandler.HandleCancelledAsync(new CancelledStatusRequest(
                invoice.SourceId.Value, 
                invoice.PaymentId.Value
            ), cancellationToken);
            
            if (!sourceHandle.IsSuccess)
                logger.LogError(
                    "Source {SourceType} {SourceId} was not void after payment {PaymentId}: Error: {Error}",
                    invoice.SourceType,
                    invoice.SourceId,
                    invoice.PaymentId,
                    sourceHandle.Errors.FirstOrDefault());
            else 
                logger.LogInformation(
                    "Source {SourceType} {SourceId} was void after payment {PaymentId}",
                    invoice.SourceType,
                    invoice.SourceId,
                    invoice.PaymentId);
        }
        else
        {
            logger.LogInformation(
                "Invoice {InvoiceId} has no source {SourceType} {SourceId} entity for handling, PaymentId: {PaymentId}",
                invoice.Id,
                invoice.SourceType,
                invoice.SourceId,
                request.PaymentId);
        }

        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}