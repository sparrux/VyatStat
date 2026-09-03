using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Subscription : AggregateRoot
{
    Subscription() { }

    Subscription(
        Guid customerId,
        Guid planId,
        string planNameSnapshot,
        Money priceSnapshot,
        BillingInterval billingInterval)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        PlanId = planId;
        PlanNameSnapshot = planNameSnapshot;
        PriceSnapshot = priceSnapshot;
        BillingInterval = billingInterval;
        Status = SubscriptionStatus.PendingPayment;
    }

    public Guid CustomerId { get; private set; }
    public Guid PlanId { get; private set; }
    public string PlanNameSnapshot { get; private set; }
    public Money PriceSnapshot { get; private set; }
    public BillingInterval BillingInterval { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DatesRange? CurrentPeriod { get; private set; }
    public DateTimeOffset? CancelledAt { get; private set; }
    public DateTimeOffset? PausedAt { get; private set; }

    public static Result<Subscription> Create(Guid customerId, SubscriptionPlan plan)
    {
        if (customerId == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        if (!plan.IsAvailable)
            return Result.Error("Subscription plan is not available");

        return Result.Success(new Subscription(
            customerId,
            plan.Id,
            plan.Name,
            plan.Price,
            plan.BillingInterval));
    }

    public Result Activate(DatesRange period)
    {
        if (Status == SubscriptionStatus.Active && CurrentPeriod == period)
            return Result.Success();

        if (Status is not (SubscriptionStatus.PendingPayment or SubscriptionStatus.Active))
            return Result.Error($"Cannot activate a subscription that is {Status}");

        Status = SubscriptionStatus.Active;
        CurrentPeriod = period;
        PausedAt = null;
        return Result.Success();
    }

    public Result Renew(DatesRange nextPeriod)
    {
        if (Status is not SubscriptionStatus.Active)
            return Result.Error("Only an active subscription can be renewed");

        CurrentPeriod = nextPeriod;
        return Result.Success();
    }

    public Result Pause()
    {
        if (Status == SubscriptionStatus.Paused)
            return Result.Success();

        if (Status is not SubscriptionStatus.Active)
            return Result.Error("Only an active subscription can be paused");

        Status = SubscriptionStatus.Paused;
        PausedAt = DateTimeOffset.UtcNow;
        return Result.Success();
    }

    public Result Resume()
    {
        if (Status == SubscriptionStatus.Active)
            return Result.Success();

        if (Status is not SubscriptionStatus.Paused)
            return Result.Error("Only a paused subscription can be resumed");

        Status = SubscriptionStatus.Active;
        PausedAt = null;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == SubscriptionStatus.Cancelled)
            return Result.Success();

        Status = SubscriptionStatus.Cancelled;
        CancelledAt = DateTimeOffset.UtcNow;
        PausedAt = null;
        return Result.Success();
    }
}
