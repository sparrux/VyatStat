using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Abstractions.Locations;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventLocation : Location
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventLocation() { }
    
    EventLocation(string? name, double lat, double lng) : base(name, lat, lng)
    {
    }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public static Result<EventLocation> Create(string? name, double lat, double lng)
    {
        return Result.Ok(new EventLocation(name, lat, lng));
    }
}