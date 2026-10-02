using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Product : AggregateRoot
{
    Product() { }

    Product(string name, Money price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        IsAvailable = true;
    }

    public string Name { get; private set; }
    public Money Price { get; private set; }
    public bool IsAvailable { get; private set; }

    public static Result<Product> Create(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Product name cannot be null or whitespace"));

        if (price.IsZero)
            return Result.Invalid(new ValidationError("Product price must be greater than zero"));

        return Result.Success(new Product(name.Trim(), price));
    }

    public Result Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Product name cannot be null or whitespace"));

        Name = name.Trim();
        return Result.Success();
    }

    public Result ChangePrice(Money price)
    {
        if (price.IsZero)
            return Result.Invalid(new ValidationError("Product price must be greater than zero"));

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
