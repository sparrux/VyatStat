using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Donation : AggregateRoot
{
    Donation() { }

    Donation(
        Money amount,
        Guid? customerId,
        bool isAnonymous,
        BusinessReference? reference)
    {
        Id = Guid.NewGuid();
        Amount = amount;
        CustomerId = customerId;
        IsAnonymous = isAnonymous;
        Reference = reference;
        Status = DonationStatus.Created;
    }

    public Guid? CustomerId { get; private set; }
    public Money Amount { get; private set; }
    public bool IsAnonymous { get; private set; }
    public BusinessReference? Reference { get; private set; }
    public DonationStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }

    public static Result<Donation> Create(
        Money amount,
        Guid? customerId = null,
        bool isAnonymous = false,
        BusinessReference? reference = null)
    {
        if (amount.IsZero)
            return Result.Invalid(new ValidationError("Donation amount must be greater than zero"));

        if (!isAnonymous && customerId is null)
            return Result.Invalid(new ValidationError("Non-anonymous donation requires a customer"));

        if (customerId == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        return Result.Success(new Donation(amount, customerId, isAnonymous, reference));
    }

    public Result AttachPayment(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            return Result.Invalid(new ValidationError("Payment id cannot be empty"));

        if (PaymentId is not null && PaymentId != paymentId)
            return Result.Error("Donation already has a different payment attached");

        if (Status is DonationStatus.Completed or DonationStatus.Cancelled)
            return Result.Error($"Cannot attach a payment when donation is {Status}");

        PaymentId = paymentId;
        if (Status == DonationStatus.Created)
            Status = DonationStatus.Pending;

        return Result.Success();
    }

    public Result Complete()
    {
        if (Status == DonationStatus.Completed)
            return Result.Success();

        if (PaymentId is null)
            return Result.Error("Donation cannot be completed without a payment");

        if (Status is DonationStatus.Cancelled)
            return Result.Error("Cancelled donation cannot be completed");

        Status = DonationStatus.Completed;
        return Result.Success();
    }

    public Result Fail()
    {
        if (Status == DonationStatus.Failed)
            return Result.Success();

        if (Status is DonationStatus.Completed or DonationStatus.Cancelled)
            return Result.Error($"Cannot fail a donation that is {Status}");

        Status = DonationStatus.Failed;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == DonationStatus.Cancelled)
            return Result.Success();

        if (Status == DonationStatus.Completed)
            return Result.Error("Completed donation cannot be cancelled");

        Status = DonationStatus.Cancelled;
        return Result.Success();
    }
}
