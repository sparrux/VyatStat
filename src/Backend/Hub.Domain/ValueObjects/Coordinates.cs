using System.Diagnostics.CodeAnalysis;
using Hub.Domain.Common;

namespace Hub.Domain.ValueObjects;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedMember.Local")]
public sealed class Coordinates : ValueObject
{
    Coordinates() { }
    
    public Coordinates(double x, double y, int epsg)
    {
        X = x;
        Y = y;
        Epsg = epsg;
    }

    public double X { get; private set; }
    public double Y { get; private set; }
    public int Epsg { get; private set; }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return X;
        yield return Y;
        yield return Epsg;
    }
}