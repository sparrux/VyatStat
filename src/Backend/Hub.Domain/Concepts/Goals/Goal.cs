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
    
    protected Goal(string title, int currentValue, int targetValue) 
        : this(title, new GoalState(currentValue, targetValue)) { }
    
    protected Goal(string title, GoalState state)
    {
        Title = title;
        State = state;
    }
    
    public string Title { get; private set; }
    public GoalState State { get; private set; }

    internal Result UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Title cannot be null or whitespace"));
        
        Title = title;
        return Result.Success();
    }

    internal Result UpdateState(GoalState state)
    {
        State = state;
        return Result.Success();
    }
}