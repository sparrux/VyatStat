using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class PaymentAttempt : Auditable
{
    PaymentAttempt() { }

    PaymentAttempt(
        Payment payment,
        ProviderName provider,
        string? providerPaymentId,
        int attemptNumber)
    {
        Id = Guid.NewGuid();
        Payment = payment;
        PaymentId = payment.Id;
        Provider = provider;
        ProviderPaymentId = providerPaymentId;
        AttemptNumber = attemptNumber;
        Status = PaymentAttemptStatus.Pending;
    }

    public Payment Payment { get; private set; }
    public Guid PaymentId { get; private set; }

    public ProviderName Provider { get; private set; }
    public string? ProviderPaymentId { get; private set; }
    public int AttemptNumber { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }
    public string? FailureCode { get; private set; }
    public string? FailureMessage { get; private set; }

    internal static Result<PaymentAttempt> Create(
        Payment payment,
        ProviderName provider,
        string? providerPaymentId,
        int attemptNumber)
    {
        if (attemptNumber < 1)
            return Result.Invalid(new ValidationError("Attempt number must be greater than zero"));

        return Result.Success(new PaymentAttempt(
            payment,
            provider,
            NormalizeProviderId(providerPaymentId),
            attemptNumber));
    }

    internal Result AssignProviderPaymentId(string providerPaymentId)
    {
        if (string.IsNullOrWhiteSpace(providerPaymentId))
            return Result.Invalid(new ValidationError("Provider payment id cannot be null or whitespace"));

        var normalized = NormalizeProviderId(providerPaymentId);
        if (ProviderPaymentId is not null &&
            !string.Equals(ProviderPaymentId, normalized, StringComparison.Ordinal))
        {
            return Result.Error("Provider payment id is already assigned");
        }

        ProviderPaymentId = normalized;
        return Result.Success();
    }

    internal Result MarkAsRequiresAction() =>
        TransitionTo(PaymentAttemptStatus.RequiresAction);

    internal Result MarkAsProcessing() =>
        TransitionTo(PaymentAttemptStatus.Processing);

    internal Result MarkAsSucceeded()
    {
        var transitioned = TransitionTo(PaymentAttemptStatus.Succeeded);
        if (!transitioned.IsSuccess)
            return transitioned;

        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    internal Result MarkAsFailed(string? failureCode, string? failureMessage)
    {
        var transitioned = TransitionTo(PaymentAttemptStatus.Failed);
        if (!transitioned.IsSuccess)
            return transitioned;

        FailureCode = failureCode;
        FailureMessage = failureMessage;
        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    internal Result MarkAsCancelled()
    {
        var transitioned = TransitionTo(PaymentAttemptStatus.Cancelled);
        if (!transitioned.IsSuccess)
            return transitioned;

        CompletedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    Result TransitionTo(PaymentAttemptStatus target)
    {
        if (Status == target)
            return Result.Success();

        var allowed = (Status, target) switch
        {
            (PaymentAttemptStatus.Pending, PaymentAttemptStatus.RequiresAction) => true,
            (PaymentAttemptStatus.Pending, PaymentAttemptStatus.Processing) => true,
            (PaymentAttemptStatus.Pending, PaymentAttemptStatus.Succeeded) => true,
            (PaymentAttemptStatus.Pending, PaymentAttemptStatus.Failed) => true,
            (PaymentAttemptStatus.Pending, PaymentAttemptStatus.Cancelled) => true,
            (PaymentAttemptStatus.RequiresAction, PaymentAttemptStatus.Processing) => true,
            (PaymentAttemptStatus.RequiresAction, PaymentAttemptStatus.Succeeded) => true,
            (PaymentAttemptStatus.RequiresAction, PaymentAttemptStatus.Failed) => true,
            (PaymentAttemptStatus.RequiresAction, PaymentAttemptStatus.Cancelled) => true,
            (PaymentAttemptStatus.Processing, PaymentAttemptStatus.Succeeded) => true,
            (PaymentAttemptStatus.Processing, PaymentAttemptStatus.Failed) => true,
            _ => false
        };

        if (!allowed)
            return Result.Error($"Cannot transition payment attempt from {Status} to {target}");

        Status = target;
        return Result.Success();
    }

    static string? NormalizeProviderId(string? providerPaymentId) =>
        string.IsNullOrWhiteSpace(providerPaymentId) ? null : providerPaymentId.Trim();
}
