using Ardalis.Result;

namespace Hub.Application.Abstractions.Payments;

public sealed record SucceededStatusRequest(Guid TargetId, Guid PaymentId);
public sealed record FailedStatusRequest(Guid TargetId, Guid PaymentId, string? Reason);
public sealed record CancelledStatusRequest(Guid TargetId, Guid PaymentId);

public interface IFinancialTargetStatusHandler
{
    Task<Result> HandleSucceededAsync(
        SucceededStatusRequest request, 
        CancellationToken cancellationToken);
    
    Task<Result> HandleFailedAsync(
        FailedStatusRequest request, 
        CancellationToken cancellationToken);
    
    Task<Result> HandleCancelledAsync(
        CancelledStatusRequest request, 
        CancellationToken cancellationToken);
}