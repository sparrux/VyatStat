using Ardalis.Result;
using Hub.Application.Features.Payments.Commands.CreateInvoice;
using Hub.Domain.Payments;

namespace Hub.Application.Abstractions.Payments;

public interface IInvoiceService
{
    Task<Result<Invoice>> FindInvoiceAsync(
        Guid customerId,
        Guid invoiceId,
        CancellationToken cancellationToken);
    
    Task<Result<Invoice>> CreateInvoiceAsync(
        CreateInvoiceRequest request, 
        CancellationToken cancellationToken);
}