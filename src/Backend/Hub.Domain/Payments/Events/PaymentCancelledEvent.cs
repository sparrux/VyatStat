using Hub.Domain.Common.DomainEvents;

namespace Hub.Domain.Payments.Events;

public sealed class PaymentCancelledEvent : DomainEvent
{
    public PaymentCancelledEvent(Guid paymentId, PaymentPurpose purpose, Guid referenceId)
    {
        PaymentId = paymentId;
        Purpose = purpose;
        ReferenceId = referenceId;
    }
    
    public Guid PaymentId { get; }
    public PaymentPurpose Purpose { get; }
    public Guid ReferenceId { get; }
}