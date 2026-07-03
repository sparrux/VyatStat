using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain;

public sealed class Location : Auditable
{
    Location() { }
    
    Location(string? name, double lat, double lng)
    {
        Name = name;
        Latitude = lat;
        Longitude = lng;
    }

    public string? Name { get; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public static Result<Location> Create(string? name, double lat, double lng)
    {
        return Result.Ok(new Location(name, lat, lng));
    }
}