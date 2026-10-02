using Ardalis.Result;
using Hub.Domain.Payments;

namespace Hub.Application.Abstractions.Payments;

public interface ICustomerService
{
    Task<Result<Customer>> GetOrCreateAsync(
        Guid customerId, 
        CancellationToken cancellationToken);
}