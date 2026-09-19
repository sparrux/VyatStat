using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record PaymentResponse(
    Guid Id,
    Guid? CustomerId,
    PaymentPurpose Purpose,
    Guid ReferenceId,
    PaymentStatus Status,
    DateTimeOffset? SucceededAt,
    DateTimeOffset? FailedAt,
    DateTimeOffset? CancelledAt,
    string? FailureReason,
    Uri? ApprovalUrl,
    ICollection<PaymentAttemptResponse> Attempts)
{
    public static PaymentResponse From(Payment payment, Uri? approvalUrl) =>
        new(
            payment.Id, 
            payment.CustomerId, 
            payment.Purpose, 
            payment.ReferenceId, 
            payment.Status, 
            payment.SucceededAt, 
            payment.FailedAt, 
            payment.CancelledAt, 
            payment.FailureReason, 
            approvalUrl,
            [.. payment.Attempts.Select(PaymentAttemptResponse.From)]
        );
}