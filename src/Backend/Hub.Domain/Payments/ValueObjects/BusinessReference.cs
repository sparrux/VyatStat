using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;

namespace Hub.Domain.Payments.ValueObjects;

public static class BusinessReferenceTypes
{
    public const string EventContribution = "EventContribution";
}

[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class BusinessReference : ValueObject
{
    BusinessReference()
    {
        Type = null!;
    }

    BusinessReference(string type, Guid id)
    {
        Type = type;
        Id = id;
    }

    public string Type { get; private set; }
    public Guid Id { get; private set; }

    public static Result<BusinessReference> Create(string type, Guid id)
    {
        if (string.IsNullOrWhiteSpace(type))
            return Result.Invalid(new ValidationError("Business reference type cannot be null or whitespace"));

        if (id == Guid.Empty)
            return Result.Invalid(new ValidationError("Business reference id cannot be empty"));

        return Result.Success(new BusinessReference(type.Trim(), id));
    }

    public static Result<BusinessReference> ForEventContribution(Guid eventId) =>
        Create(BusinessReferenceTypes.EventContribution, eventId);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Type;
        yield return Id;
    }
}
