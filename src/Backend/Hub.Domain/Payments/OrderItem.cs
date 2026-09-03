using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class OrderItem : Entity
{
    OrderItem() { }

    OrderItem(
        Order order,
        Guid productId,
        string productNameSnapshot,
        int quantity,
        Money unitPriceSnapshot,
        Money lineTotal)
    {
        Id = Guid.NewGuid();
        Order = order;
        OrderId = order.Id;
        ProductId = productId;
        ProductNameSnapshot = productNameSnapshot;
        Quantity = quantity;
        UnitPriceSnapshot = unitPriceSnapshot;
        LineTotal = lineTotal;
    }

    public Order Order { get; private set; }
    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }
    public string ProductNameSnapshot { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPriceSnapshot { get; private set; }
    public Money LineTotal { get; private set; }

    internal static Result<OrderItem> Create(Order order, Product product, int quantity)
    {
        if (quantity < 1)
            return Result.Invalid(new ValidationError("Order item quantity must be at least 1"));

        if (!product.IsAvailable)
            return Result.Error($"Product '{product.Name}' is not available");

        var lineTotal = product.Price.Multiply(quantity);
        if (!lineTotal.IsSuccess)
            return lineTotal.Map();

        return Result.Success(new OrderItem(
            order,
            product.Id,
            product.Name,
            quantity,
            product.Price,
            lineTotal.Value));
    }
}
