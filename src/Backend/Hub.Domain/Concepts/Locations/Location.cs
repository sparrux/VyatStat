using Hub.Domain.Common;
using Hub.Domain.ValueObjects;

namespace Hub.Domain.Concepts.Locations;

public abstract class Location : Entity
{
    protected Location() { }
    
    protected Location(string? name, double lat, double lng)
        : this(name, new Coordinates(lat, lng)) { }
    
    protected Location(string? name, Coordinates coordinates)
    {
        Name = name;
        Coordinates = coordinates;
    }

    public string? Name { get; private set; }
    public Coordinates Coordinates { get; private set; }
}