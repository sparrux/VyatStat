using Hub.Domain.Common.DomainEvents;

namespace Hub.Domain.Payments.Events;

public sealed class PaymentFailedEvent : DomainEvent
{
    public PaymentFailedEvent(
        Guid paymentId,
        PaymentPurpose purpose,
        Guid referenceId,
        string? reason)
    {
        PaymentId = paymentId;
        Purpose = purpose;
        ReferenceId = referenceId;
        Reason = reason;
    }

    public Guid PaymentId { get; }
    public PaymentPurpose Purpose { get; }
    public Guid ReferenceId { get; }
    public string? Reason { get; }
}
