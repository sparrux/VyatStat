using Ardalis.Result;
using Hub.Application.Features.Payments.Contracts;
using Hub.Domain.Payments;

namespace Hub.Application.Abstractions.Payments;

public sealed record CreatePaymentResult(
    Payment Payment
);

public sealed record PaymentCheckoutResult(
    Payment Payment, 
    Uri? ApprovalUrl
);

public interface IPaymentService
{
    Task<Result<Payment>> FindPaymentAsync(
        Guid? referenceId, 
        string idempotencyKey, 
        CancellationToken cancellationToken);
    
    Task<Result<CreatePaymentResult>> CreatePaymentAsync(
        CreatePaymentRequest request, 
        CancellationToken cancellationToken);
    
    Task<Result<PaymentCheckoutResult>> StartCheckoutAsync(
        Payment payment,
        PaymentCheckoutRequest request, 
        CancellationToken cancellationToken);
}