using Hub.Domain.Common;
using Hub.Domain.ValueObjects;

namespace Hub.Domain.Concepts.Locations;

public abstract class Location : Entity
{
    protected Location() { }
    
    protected Location(string? name, double x, double y, int epsg)
        : this(name, new Coordinates(x, y, epsg)) { }
    
    protected Location(string? name, Coordinates coordinates)
    {
        Name = name;
        Coordinates = coordinates;
    }

    public string? Name { get; protected set; }
    public Coordinates Coordinates { get; protected set; }
}