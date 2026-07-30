using Ardalis.Result;
using Hub.Domain.Common;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Concepts.Roles;

public abstract class Role : Auditable
{
    protected Role() { }

    protected Role(string name, bool isSealed)
    {
        Name = name;
        IsSealed = isSealed;
    }
    
    public string Name { get; private set; }
    public bool IsSealed { get; private set; }
    
    internal Result UpdateName(string name)
    {
        if (IsSealed)
            return Result.Error("Role cannot be changed because sealed");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Role name is required"));

        Name = name;
        return Result.Success();
    }
}