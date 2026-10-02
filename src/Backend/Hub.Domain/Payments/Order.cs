using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

public sealed record OrderLineDraft(Product Product, int Quantity);

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class Order : AggregateRoot
{
    readonly List<OrderItem> _items = [];

    Order() { }

    Order(Guid customerId, Currency currency)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Created;
        Total = Money.Zero(currency);
    }

    public Guid CustomerId { get; private set; }
    public Money Total { get; private set; }
    public OrderStatus Status { get; private set; }
    public Guid? PaymentId { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    public static Result<Order> Place(Guid customerId, IReadOnlyCollection<OrderLineDraft> lines)
    {
        if (customerId == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        if (lines.Count == 0)
            return Result.Invalid(new ValidationError("Order must contain at least one item"));

        var currencies = lines.Select(x => x.Product.Price.Currency).Distinct().ToList();
        if (currencies.Count > 1)
            return Result.Error("Order items must use the same currency");

        var order = new Order(customerId, currencies[0]);

        foreach (var line in lines)
        {
            var item = OrderItem.Create(order, line.Product, line.Quantity);
            if (!item.IsSuccess)
                return item.Map();

            order._items.Add(item.Value);
        }

        var total = Money.Zero(currencies[0]);
        foreach (var item in order._items)
        {
            var added = total.Add(item.LineTotal);
            if (!added.IsSuccess)
                return added.Map();

            total = added.Value;
        }

        if (total.IsZero)
            return Result.Invalid(new ValidationError("Order total must be greater than zero"));

        order.Total = total;
        return Result.Success(order);
    }

    public Result AttachPayment(Guid paymentId)
    {
        if (paymentId == Guid.Empty)
            return Result.Invalid(new ValidationError("Payment id cannot be empty"));

        if (PaymentId is not null && PaymentId != paymentId)
            return Result.Error("Order already has a different payment attached");

        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
            return Result.Error($"Cannot attach a payment when order is {Status}");

        PaymentId = paymentId;
        if (Status == OrderStatus.Created)
            Status = OrderStatus.AwaitingPayment;

        return Result.Success();
    }

    public Result MarkAsPaid()
    {
        if (Status is OrderStatus.Paid or OrderStatus.Completed)
            return Result.Success();

        if (PaymentId is null)
            return Result.Error("Order cannot be marked as paid without a payment");

        if (Status == OrderStatus.Cancelled)
            return Result.Error("Cancelled order cannot be marked as paid");

        Status = OrderStatus.Paid;
        return Result.Success();
    }

    public Result Complete()
    {
        if (Status == OrderStatus.Completed)
            return Result.Success();

        if (Status is not OrderStatus.Paid)
            return Result.Error("Only a paid order can be completed");

        Status = OrderStatus.Completed;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Success();

        if (Status is OrderStatus.Paid or OrderStatus.Completed)
            return Result.Error("Paid order cannot be cancelled");

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }
}
