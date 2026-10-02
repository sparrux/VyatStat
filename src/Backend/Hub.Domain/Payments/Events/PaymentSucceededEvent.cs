using Hub.Domain.Common.DomainEvents;
using Hub.Domain.Payments.ValueObjects;

namespace Hub.Domain.Payments.Events;

public sealed class PaymentSucceededEvent : DomainEvent
{
    public PaymentSucceededEvent(
        Guid paymentId,
        Guid? customerId,
        PaymentPurpose purpose,
        Guid referenceId,
        Money amount)
    {
        PaymentId = paymentId;
        CustomerId = customerId;
        Purpose = purpose;
        ReferenceId = referenceId;
        Amount = amount;
    }

    public Guid PaymentId { get; }
    public Guid? CustomerId { get; }
    public PaymentPurpose Purpose { get; }
    public Guid ReferenceId { get; }
    public Money Amount { get; }
}
