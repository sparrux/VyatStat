using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Locations;
using Hub.Domain.ValueObjects;

namespace Hub.Domain.Presets;

public sealed class LocationPreset : Location
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    LocationPreset() { }
    
    LocationPreset(string? name, double lat, double lng) : base(name, lat, lng)
    {
    }

    LocationPreset(string? name, Coordinates coordinates) : base(name, coordinates)
    {
    }

    public static Result<LocationPreset> Create(string? name, Coordinates coordinates)
    {
        return Result.Success(new LocationPreset(name, coordinates));
    }
}