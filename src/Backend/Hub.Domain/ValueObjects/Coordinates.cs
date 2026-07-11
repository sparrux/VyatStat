using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;

namespace Hub.Domain.ValueObjects;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Coordinates : ValueObject
{
    Coordinates() { }
    
    public Coordinates(double lat, double lng)
    {
        Latitude = lat;
        Longitude = lng;
    }

    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}