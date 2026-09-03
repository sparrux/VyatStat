using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

namespace Hub.Domain.Payments.ValueObjects;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
public sealed class ProviderName : ValueObject
{
    ProviderName()
    {
        Value = null!;
    }

    ProviderName(string value)
    {
        Value = value;
    }

    public string Value { get; private set; }

    public static Result<ProviderName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Invalid(new ValidationError("Provider name cannot be null or whitespace"));

        var normalized = value.Trim();
        if (normalized.Length > 50)
            return Result.Invalid(new ValidationError("Provider name cannot exceed 50 characters"));

        return Result.Success(new ProviderName(normalized));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value.ToUpperInvariant();
    }

    public override string ToString() => Value;
}
