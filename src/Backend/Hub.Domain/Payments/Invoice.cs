using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Invoice : AggregateRoot
{
    Invoice() { }

    Invoice(
        Guid customerId,
        Money amount,
        DateTimeOffset dueDate,
        Guid? subscriptionId,
        DatesRange? billingPeriod)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Amount = amount;
        DueDate = dueDate;
        SubscriptionId = subscriptionId;
        BillingPeriod = billingPeriod;
        Status = InvoiceStatus.Open;
    }

    public Guid CustomerId { get; private set; }
    public Guid? SubscriptionId { get; private set; }
    public DatesRange? BillingPeriod { get; private set; }
    public Money Amount { get; private set; }
    public DateTimeOffset DueDate { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }

    public static Result<Invoice> Issue(
        Guid customerId,
        Money amount,
        DateTimeOffset dueDate,
        Guid? subscriptionId = null,
        DatesRange? billingPeriod = null)
    {
        if (customerId == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        if (amount.IsZero)
            return Result.Invalid(new ValidationError("Invoice amount must be greater than zero"));

        return Result.Success(new Invoice(customerId, amount, dueDate, subscriptionId, billingPeriod));
    }

    public static Result<Invoice> IssueForSubscription(
        Subscription subscription,
        DatesRange billingPeriod,
        DateTimeOffset dueDate)
    {
        if (subscription.Status == SubscriptionStatus.Cancelled)
            return Result.Error("Cannot issue an invoice for a cancelled subscription");

        return Issue(
            subscription.CustomerId,
            subscription.PriceSnapshot,
            dueDate,
            subscription.Id,
            billingPeriod);
    }

    public Result AttachPayment(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            return Result.Invalid(new ValidationError("Payment id cannot be empty"));

        if (PaymentId is not null && PaymentId != paymentId)
            return Result.Error("Invoice already has a different payment attached");

        if (Status is InvoiceStatus.Paid or InvoiceStatus.Void)
            return Result.Error($"Cannot attach a payment when invoice is {Status}");

        PaymentId = paymentId;
        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status == InvoiceStatus.Paid)
            return Result.Success();

        if (PaymentId is null)
            return Result.Error("Invoice cannot be marked as paid without a payment");

        if (Status == InvoiceStatus.Void)
            return Result.Error("Void invoice cannot be marked as paid");

        Status = InvoiceStatus.Paid;
        return Result.Success();
    }

    public Result MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Overdue)
            return Result.Success();

        if (Status is not InvoiceStatus.Open)
            return Result.Error("Only an open invoice can be marked as overdue");

        Status = InvoiceStatus.Overdue;
        return Result.Success();
    }

    public Result Void()
    {
        if (Status == InvoiceStatus.Void)
            return Result.Success();

        if (Status == InvoiceStatus.Paid)
            return Result.Error("Paid invoice cannot be voided");

        Status = InvoiceStatus.Void;
        return Result.Success();
    }
}
