using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.Events;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
public sealed class Payment : AggregateRoot
{
    readonly List<PaymentAttempt> _attempts = [];
    readonly List<Refund> _refunds = [];

    Payment() { }

    Payment(
        Money amount,
        PaymentPurpose purpose,
        Guid referenceId,
        Guid? customerId,
        string? idempotencyKey)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        Purpose = purpose;
        ReferenceId = referenceId;
        CustomerId = customerId;
        IdempotencyKey = idempotencyKey;
        Status = PaymentStatus.Created;
    }

    public Guid? CustomerId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentPurpose Purpose { get; private set; }
    public Guid ReferenceId { get; private set; }
    public string? IdempotencyKey { get; private set; }
    public PaymentStatus Status { get; private set; }

    public DateTimeOffset? SucceededAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public string? FailureReason { get; private set; }

    public IReadOnlyCollection<PaymentAttempt> Attempts => _attempts;
    public IReadOnlyCollection<Refund> Refunds => _refunds;

    public Money RefundedAmount => SumRefunds(RefundStatus.Succeeded);
    public Money RemainingRefundable
    {
        get
        {
            var remaining = Amount.Subtract(SumReservedRefunds());
            return remaining.IsSuccess ? remaining.Value : Money.Zero(Amount.Currency);
        }
    }

    public static Result<Payment> Create(
        Money amount,
        PaymentPurpose purpose,
        Guid referenceId,
        Guid? customerId = null,
        string? idempotencyKey = null)
    {
        if (amount.IsZero)
            return Result.Invalid(new ValidationError("Payment amount must be greater than zero"));

        if (referenceId == Guid.Empty)
            return Result.Invalid(new ValidationError("Payment reference id cannot be empty"));

        if (customerId == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        if (!Enum.IsDefined(purpose))
            return Result.Invalid(new ValidationError("Payment purpose is not defined"));

        var key = string.IsNullOrWhiteSpace(idempotencyKey) ? null : idempotencyKey.Trim();

        return Result.Success(new Payment(amount, purpose, referenceId, customerId, key));
    }

    public Result<PaymentAttempt> StartAttempt(ProviderName provider, string? providerPaymentId = null)
    {
        if (Status is PaymentStatus.Succeeded or PaymentStatus.Cancelled)
            return Result.Error($"Cannot start a payment attempt when payment is {Status}");

        var attempt = PaymentAttempt.Create(this, provider, providerPaymentId, _attempts.Count + 1);
        if (!attempt.IsSuccess)
            return attempt;

        _attempts.Add(attempt.Value);

        if (Status is PaymentStatus.Created or PaymentStatus.Failed)
            Status = PaymentStatus.Pending;

        return attempt;
    }

    public Result AssignAttemptProviderId(Guid attemptId, string providerPaymentId)
    {
        var attempt = FindAttempt(attemptId);
        if (attempt is null)
            return Result.NotFound("Payment attempt not found");

        return attempt.AssignProviderPaymentId(providerPaymentId);
    }

    public Result MarkAttemptRequiresAction(Guid attemptId)
    {
        var attempt = FindAttempt(attemptId);
        if (attempt is null)
            return Result.NotFound("Payment attempt not found");

        var marked = attempt.MarkAsRequiresAction();
        if (!marked.IsSuccess)
            return marked;

        return TransitionTo(PaymentStatus.RequiresAction);
    }

    public Result MarkAttemptProcessing(Guid attemptId)
    {
        var attempt = FindAttempt(attemptId);
        if (attempt is null)
            return Result.NotFound("Payment attempt not found");

        var marked = attempt.MarkAsProcessing();
        if (!marked.IsSuccess)
            return marked;

        return TransitionTo(PaymentStatus.Processing);
    }

    public Result SucceedAttempt(Guid attemptId)
    {
        var attempt = FindAttempt(attemptId);
        if (attempt is null)
            return Result.NotFound("Payment attempt not found");

        var marked = attempt.MarkAsSucceeded();
        if (!marked.IsSuccess)
            return marked;

        return MarkAsSucceeded();
    }

    public Result FailAttempt(Guid attemptId, string? failureCode, string? failureMessage)
    {
        var attempt = FindAttempt(attemptId);
        if (attempt is null)
            return Result.NotFound("Payment attempt not found");

        var marked = attempt.MarkAsFailed(failureCode, failureMessage);
        if (!marked.IsSuccess)
            return marked;

        return MarkAsFailed(failureMessage);
    }

    public Result Cancel()
    {
        if (Status is PaymentStatus.Succeeded)
            return Result.Error("Succeeded payment cannot be cancelled");

        if (Status is PaymentStatus.Cancelled)
            return Result.Success();

        foreach (var attempt in _attempts.Where(x =>
                     x.Status is PaymentAttemptStatus.Pending
                         or PaymentAttemptStatus.RequiresAction
                         or PaymentAttemptStatus.Processing))
        {
            var cancelled = attempt.MarkAsCancelled();
            if (!cancelled.IsSuccess)
                return cancelled;
        }

        var transitioned = TransitionTo(PaymentStatus.Cancelled);
        if (!transitioned.IsSuccess)
            return transitioned;

        CancelledAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result<Refund> RequestRefund(Money amount)
    {
        if (Status is not PaymentStatus.Succeeded)
            return Result.Error("Only a succeeded payment can be refunded");

        if (!Amount.IsSameCurrency(amount))
            return Result.Error("Refund currency must match the payment currency");

        if (amount.IsZero)
            return Result.Invalid(new ValidationError("Refund amount must be greater than zero"));

        if (amount.Amount > RemainingRefundable.Amount)
            return Result.Error("Refund amount exceeds the remaining refundable balance");

        var refund = Refund.Create(this, amount);
        if (!refund.IsSuccess)
            return refund;

        _refunds.Add(refund.Value);
        return refund;
    }

    public Result AssignRefundProviderId(Guid refundId, string providerRefundId)
    {
        var refund = FindRefund(refundId);
        if (refund is null)
            return Result.NotFound("Refund not found");

        return refund.AssignProviderRefundId(providerRefundId);
    }

    public Result MarkRefundProcessing(Guid refundId)
    {
        var refund = FindRefund(refundId);
        if (refund is null)
            return Result.NotFound("Refund not found");

        return refund.MarkAsProcessing();
    }

    public Result ConfirmRefund(Guid refundId)
    {
        var refund = FindRefund(refundId);
        if (refund is null)
            return Result.NotFound("Refund not found");

        var alreadySucceeded = refund.Status == RefundStatus.Succeeded;
        var confirmed = refund.MarkAsSucceeded();
        if (!confirmed.IsSuccess)
            return confirmed;

        if (!alreadySucceeded)
            AddDomainEvent(new RefundSucceededEvent(Id, refund.Id, refund.Amount));

        return Result.Success();
    }

    public Result FailRefund(Guid refundId, string? reason)
    {
        var refund = FindRefund(refundId);
        if (refund is null)
            return Result.NotFound("Refund not found");

        return refund.MarkAsFailed(reason);
    }

    Result MarkAsSucceeded()
    {
        if (Status == PaymentStatus.Succeeded)
            return Result.Success();

        var transitioned = TransitionTo(PaymentStatus.Succeeded);
        if (!transitioned.IsSuccess)
            return transitioned;

        SucceededAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new PaymentSucceededEvent(Id, CustomerId, Purpose, ReferenceId, Amount));
        return Result.Success();
    }

    Result MarkAsFailed(string? reason)
    {
        if (Status == PaymentStatus.Failed)
            return Result.Success();

        var transitioned = TransitionTo(PaymentStatus.Failed);
        if (!transitioned.IsSuccess)
            return transitioned;

        FailureReason = reason;
        FailedAt = DateTimeOffset.UtcNow;
        AddDomainEvent(new PaymentFailedEvent(Id, Purpose, ReferenceId, reason));
        return Result.Success();
    }

    Result TransitionTo(PaymentStatus target)
    {
        if (Status == target)
            return Result.Success();

        var allowed = (Status, target) switch
        {
            (PaymentStatus.Created, PaymentStatus.Pending) => true,
            (PaymentStatus.Created, PaymentStatus.Cancelled) => true,
            (PaymentStatus.Pending, PaymentStatus.RequiresAction) => true,
            (PaymentStatus.Pending, PaymentStatus.Processing) => true,
            (PaymentStatus.Pending, PaymentStatus.Succeeded) => true,
            (PaymentStatus.Pending, PaymentStatus.Failed) => true,
            (PaymentStatus.Pending, PaymentStatus.Cancelled) => true,
            (PaymentStatus.RequiresAction, PaymentStatus.Processing) => true,
            (PaymentStatus.RequiresAction, PaymentStatus.Succeeded) => true,
            (PaymentStatus.RequiresAction, PaymentStatus.Failed) => true,
            (PaymentStatus.RequiresAction, PaymentStatus.Cancelled) => true,
            (PaymentStatus.Processing, PaymentStatus.Succeeded) => true,
            (PaymentStatus.Processing, PaymentStatus.Failed) => true,
            (PaymentStatus.Failed, PaymentStatus.Pending) => true,
            _ => false
        };

        if (!allowed)
            return Result.Error($"Cannot transition payment from {Status} to {target}");

        Status = target;
        return Result.Success();
    }

    PaymentAttempt? FindAttempt(Guid attemptId) =>
        _attempts.FirstOrDefault(x => x.Id == attemptId);

    Refund? FindRefund(Guid refundId) =>
        _refunds.FirstOrDefault(x => x.Id == refundId);

    Money SumRefunds(params RefundStatus[] statuses)
    {
        var total = _refunds
            .Where(x => statuses.Contains(x.Status))
            .Sum(x => x.Amount.Amount);

        return Money.Create(total, Amount.Currency).Value;
    }

    Money SumReservedRefunds() =>
        SumRefunds(RefundStatus.Pending, RefundStatus.Processing, RefundStatus.Succeeded);
}
