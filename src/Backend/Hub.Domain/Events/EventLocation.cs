using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Locations;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventLocation : Location
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventLocation() { }
    
    EventLocation(string? name, double lat, double lng) : base(name, lat, lng)
    {
    }
    
    EventLocation(string? name, Coordinates coordinates) : base(name, coordinates)
    {
    }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    internal static Result<EventLocation> Create(string? name, Coordinates coordinates) => 
        Result.Success(new EventLocation(name, coordinates));
}