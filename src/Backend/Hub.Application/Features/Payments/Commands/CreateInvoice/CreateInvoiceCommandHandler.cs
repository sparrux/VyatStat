using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;
using Hub.Domain.Payments.ValueObjects;
using Hub.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Commands.CreateInvoice;

sealed class CreateInvoiceCommandHandler(
    IHubDbContext hubDbContext,
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<CreateInvoiceCommand, InvoiceResponse>
{
    public async Task<Result<InvoiceResponse>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var userExists = await hubDbContext.Users
            .AnyAsync(x => x.Id == command.Request.CustomerId, cancellationToken);

        if (!userExists) return Result.NotFound("User not found by customer id");

        var customer = await GetOrCreateCustomer(command.Request.CustomerId, cancellationToken);
        if (!customer.IsSuccess) return customer.Map();

        var request = command.Request;
        
        var money = Money.Create(request.Amount, request.Currency);
        if (!money.IsSuccess) return money.Map();

        DatesRange? billingPeriod = null;
        
        if (request.BillingPeriod is not null)
            billingPeriod = new DatesRange(request.BillingPeriod.StartDate, request.BillingPeriod.EndDate);
        
        var invoice = Invoice.Issue(
            customer.Value.Id,
            money.Value,
            request.DueDate,
            subscriptionId: null,
            billingPeriod
        );
        if (!invoice.IsSuccess) return invoice.Map();

        await paymentsDbContext.Invoices.AddAsync(invoice.Value, cancellationToken);
        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(InvoiceResponse.From(invoice.Value));
    }
    
    async Task<Result<Customer>> GetOrCreateCustomer(Guid userId, CancellationToken cancellationToken)
    {
        var customer = await paymentsDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (customer is not null)
            return Result.Success(customer);

        var created = Customer.Create(userId);
        if (!created.IsSuccess)
            return created;

        await paymentsDbContext.Customers.AddAsync(created.Value, cancellationToken);
        return created;
    }
}