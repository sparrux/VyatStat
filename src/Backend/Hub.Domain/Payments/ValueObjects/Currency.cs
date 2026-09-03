using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

namespace Hub.Domain.Payments.ValueObjects;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
public sealed class Currency : ValueObject
{
    Currency()
    {
        Code = null!;
    }

    Currency(string code)
    {
        Code = code;
    }

    public string Code { get; private set; }

    public static Currency Rub { get; } = new("RUB");
    public static Currency Usd { get; } = new("USD");
    public static Currency Eur { get; } = new("EUR");

    public static Result<Currency> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Invalid(new ValidationError("Currency code cannot be null or whitespace"));

        var normalized = code.Trim().ToUpperInvariant();
        if (normalized.Length != 3 || !normalized.All(char.IsLetter))
            return Result.Invalid(new ValidationError("Currency code must be a 3-letter ISO 4217 value"));

        return Result.Success(new Currency(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }

    public override string ToString() => Code;
}
