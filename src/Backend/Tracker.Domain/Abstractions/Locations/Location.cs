using Tracker.Domain.Common;

namespace Tracker.Domain.Abstractions.Locations;

public abstract class Location : Auditable
{
    protected Location() { }
    
    protected Location(string? name, double lat, double lng)
    {
        Name = name;
        Latitude = lat;
        Longitude = lng;
    }

    public string? Name { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
}