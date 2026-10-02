using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

namespace Hub.Domain.Payments.ValueObjects;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
public sealed class Money : ValueObject
{
    Money()
    {
        Currency = null!;
    }

    Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public bool IsZero => Amount == 0;

    public static Result<Money> Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            return Result.Invalid(new ValidationError("Money amount cannot be negative"));

        return Result.Success(new Money(decimal.Round(amount, 4, MidpointRounding.AwayFromZero), currency));
    }

    public static Result<Money> Create(decimal amount, string currencyCode)
    {
        var currency = Currency.Create(currencyCode);
        if (!currency.IsSuccess)
            return currency.Map();

        return Create(amount, currency.Value);
    }

    public static Money Zero(Currency currency) => new(0, currency);

    public Result<Money> Add(Money other)
    {
        if (Currency != other.Currency)
            return Result.Error("Cannot add money with different currencies");

        return Create(Amount + other.Amount, Currency);
    }

    public Result<Money> Subtract(Money other)
    {
        if (Currency != other.Currency)
            return Result.Error("Cannot subtract money with different currencies");

        return Create(Amount - other.Amount, Currency);
    }

    public Result<Money> Multiply(int quantity)
    {
        if (quantity < 0)
            return Result.Invalid(new ValidationError("Quantity cannot be negative"));

        return Create(Amount * quantity, Currency);
    }

    public bool IsSameCurrency(Money other) => Currency == other.Currency;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount} {Currency.Code}";
}
