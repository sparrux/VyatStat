using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class Refund : Auditable
{
    Refund() { }

    Refund(Payment payment, Money amount)
    {
        Id = Guid.NewGuid();
        Payment = payment;
        PaymentId = payment.Id;
        Amount = amount;
        Status = RefundStatus.Pending;
    }

    public Payment Payment { get; private set; }
    public Guid PaymentId { get; private set; }

    public Money Amount { get; private set; }
    public RefundStatus Status { get; private set; }
    public string? ProviderRefundId { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    internal static Result<Refund> Create(Payment payment, Money amount)
    {
        if (amount.IsZero)
            return Result.Invalid(new ValidationError("Refund amount must be greater than zero"));

        return Result.Success(new Refund(payment, amount));
    }

    internal Result AssignProviderRefundId(string providerRefundId)
    {
        if (string.IsNullOrWhiteSpace(providerRefundId))
            return Result.Invalid(new ValidationError("Provider refund id cannot be null or whitespace"));

        var normalized = providerRefundId.Trim();
        if (ProviderRefundId is not null &&
            !string.Equals(ProviderRefundId, normalized, StringComparison.Ordinal))
        {
            return Result.Error("Provider refund id is already assigned");
        }

        ProviderRefundId = normalized;
        return Result.Success();
    }

    internal Result MarkAsProcessing() => TransitionTo(RefundStatus.Processing);

    internal Result MarkAsSucceeded()
    {
        var transitioned = TransitionTo(RefundStatus.Succeeded);
        if (!transitioned.IsSuccess)
            return transitioned;

        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    internal Result MarkAsFailed(string? reason)
    {
        var transitioned = TransitionTo(RefundStatus.Failed);
        if (!transitioned.IsSuccess)
            return transitioned;

        FailureReason = reason;
        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    internal Result MarkAsCancelled()
    {
        var transitioned = TransitionTo(RefundStatus.Cancelled);
        if (!transitioned.IsSuccess)
            return transitioned;

        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    Result TransitionTo(RefundStatus target)
    {
        if (Status == target)
            return Result.Success();

        var allowed = (Status, target) switch
        {
            (RefundStatus.Pending, RefundStatus.Processing) => true,
            (RefundStatus.Pending, RefundStatus.Succeeded) => true,
            (RefundStatus.Pending, RefundStatus.Failed) => true,
            (RefundStatus.Pending, RefundStatus.Cancelled) => true,
            (RefundStatus.Processing, RefundStatus.Succeeded) => true,
            (RefundStatus.Processing, RefundStatus.Failed) => true,
            _ => false
        };

        if (!allowed)
            return Result.Error($"Cannot transition refund from {Status} to {target}");

        Status = target;
        return Result.Success();
    }
}
