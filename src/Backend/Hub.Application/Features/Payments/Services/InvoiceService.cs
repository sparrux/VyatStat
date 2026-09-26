using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Payments.Commands.CreateInvoice;
using Hub.Domain.Payments;
using Hub.Domain.Payments.ValueObjects;
using Hub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Services;

sealed class InvoiceService(
    IPaymentsDbContext paymentsDbContext
) : IInvoiceService
{
    public async Task<Result<Invoice>> FindInvoiceAsync(
        Guid customerId, 
        Guid invoiceId,
        CancellationToken cancellationToken)
    {
        var invoice = await paymentsDbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == invoiceId && x.CustomerId == customerId, cancellationToken);

        return invoice is null 
            ? Result.NotFound("Invoice not found") 
            : invoice;
    }

    public async Task<Result<Invoice>> CreateInvoiceAsync(
        CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        var money = Money.Create(request.Amount, request.Currency);
        if (!money.IsSuccess)
            return money.Map();

        DatesRange? billingPeriod = null;
        
        if (request.BillingPeriod != null)
            billingPeriod = new DatesRange(request.BillingPeriod.StartDate, request.BillingPeriod.EndDate);
        
        var invoice = Invoice.Issue(
            request.CustomerId,
            money,
            request.DueDate,
            request.SourceId,
            request.SourceType,
            billingPeriod
        );
        if (!invoice.IsSuccess)
            return invoice;

        await paymentsDbContext.Invoices.AddAsync(invoice, cancellationToken);
        await paymentsDbContext.SaveChangesAsync(cancellationToken);

        return invoice;
    }
}