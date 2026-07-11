using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;
using Hub.Domain.Common.Exceptions;

namespace Hub.Domain.ValueObjects;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class DatesRange : ValueObject
{
    DatesRange() { }
    
    public DatesRange(DateTimeOffset start, DateTimeOffset end)
    {
        if (start > end)
            throw new DomainException("Start date must be before end date");
        
        StartDate = start;
        EndDate = end;
    }
    
    public DateTimeOffset EndDate { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }
}