using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Concepts.Goals;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public abstract class Goal : Auditable
{
    protected Goal() { }
    
    protected Goal(string name)
    {
        Name = name;
    }
    
    public string Name { get; protected set; }
}