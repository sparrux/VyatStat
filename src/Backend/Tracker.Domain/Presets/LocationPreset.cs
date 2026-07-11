using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using FluentResults;
using Tracker.Domain.Abstractions;
using Tracker.Domain.Abstractions.Locations;

namespace Tracker.Domain.Presets;

public sealed class LocationPreset : Location
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    LocationPreset() { }
    
    LocationPreset(string? name, double lat, double lng) : base(name, lat, lng)
    {
    }
    
    public static Result<LocationPreset> Create(string? name, double lat, double lng)
    {
        return Result.Ok(new LocationPreset(name, lat, lng));
    }
}