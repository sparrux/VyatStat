using Hub.Domain.Common.DomainEvents;
using Hub.Domain.Payments.ValueObjects;

namespace Hub.Domain.Payments.Events;

public sealed class RefundSucceededEvent : DomainEvent
{
    public RefundSucceededEvent(Guid paymentId, Guid refundId, Money amount)
    {
        PaymentId = paymentId;
        RefundId = refundId;
        Amount = amount;
    }

    public Guid PaymentId { get; }
    public Guid RefundId { get; }
    public Money Amount { get; }
}
