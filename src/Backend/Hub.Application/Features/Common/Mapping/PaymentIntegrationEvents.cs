using Hub.Application.Abstractions.Messaging;
using Hub.Application.Features.Payments.Messages;
using Hub.Domain.Common.DomainEvents;
using Hub.Domain.Payments.Events;

namespace Hub.Application.Features.Common.Mapping;

static class PaymentIntegrationEvents
{
    public static IIntegrationEvent? From(this IDomainEvent domainEvent) => 
        domainEvent switch
        {
            PaymentSucceededEvent succeeded => new PaymentSucceeded(
                succeeded.Id,
                succeeded.PaymentId,
                succeeded.CustomerId,
                succeeded.Purpose.ToString(),
                succeeded.ReferenceId,
                succeeded.Amount.Amount,
                succeeded.Amount.Currency.Code,
                succeeded.OccurredOn),
            
            PaymentFailedEvent failed => new PaymentFailed(
                failed.Id,
                failed.PaymentId,
                failed.Purpose.ToString(),
                failed.ReferenceId,
                failed.Reason,
                failed.OccurredOn),
            
            PaymentCancelledEvent cancelled => new PaymentCancelled(
                cancelled.Id,
                cancelled.PaymentId,
                cancelled.Purpose.ToString(),
                cancelled.ReferenceId,
                cancelled.OccurredOn),
            
            RefundSucceededEvent refunded => new RefundSucceeded(
                refunded.Id,
                refunded.PaymentId,
                refunded.RefundId,
                refunded.Amount.Amount,
                refunded.Amount.Currency.Code,
                refunded.OccurredOn),
            
            _ => null
        };
}