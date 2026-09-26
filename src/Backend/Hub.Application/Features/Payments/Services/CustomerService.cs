using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Domain.Payments;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Services;

sealed class CustomerService(IPaymentsDbContext paymentsDbContext) : ICustomerService
{
    public async Task<Result<Customer>> GetOrCreateAsync(
        Guid customerId, 
        CancellationToken cancellationToken)
    {
        var customer = await paymentsDbContext.Customers
            .FirstOrDefaultAsync(x => x.Id == customerId, cancellationToken);
        
        if (customer is not null)
            return Result.Success(customer);

        var created = Customer.Create(customerId);
        if (!created.IsSuccess)
            return created;

        await paymentsDbContext.Customers.AddAsync(created.Value, cancellationToken);
        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        
        return created;
    }
}