using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class SubscriptionPlan : AggregateRoot
{
    SubscriptionPlan() { }

    SubscriptionPlan(string name, Money price, BillingInterval interval, string entitlementKey)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        BillingInterval = interval;
        EntitlementKey = entitlementKey;
        IsAvailable = true;
    }

    public string Name { get; private set; }
    public Money Price { get; private set; }
    public BillingInterval BillingInterval { get; private set; }
    public string EntitlementKey { get; private set; }
    public bool IsAvailable { get; private set; }

    public static Result<SubscriptionPlan> Create(
        string name,
        Money price,
        BillingInterval interval,
        string entitlementKey)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Subscription plan name cannot be null or whitespace"));

        if (price.IsZero)
            return Result.Invalid(new ValidationError("Subscription plan price must be greater than zero"));

        if (!Enum.IsDefined(interval))
            return Result.Invalid(new ValidationError("Billing interval is not defined"));

        if (string.IsNullOrWhiteSpace(entitlementKey))
            return Result.Invalid(new ValidationError("Entitlement key cannot be null or whitespace"));

        return Result.Success(new SubscriptionPlan(
            name.Trim(),
            price,
            interval,
            entitlementKey.Trim()));
    }

    public Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Subscription plan name cannot be null or whitespace"));

        Name = name.Trim();
        return Result.Success();
    }

    public Result ChangePrice(Money price)
    {
        if (price.IsZero)
            return Result.Invalid(new ValidationError("Subscription plan price must be greater than zero"));

        Price = price;
        return Result.Success();
    }

    public Result Discontinue()
    {
        IsAvailable = false;
        return Result.Success();
    }

    public Result MakeAvailable()
    {
        IsAvailable = true;
        return Result.Success();
    }
}
