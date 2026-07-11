using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Common;
using Hub.Domain.Validators;
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

    public Result UpdateTitle(string title)
    {
        var titleValidation = new GoalTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());
        
        Title = title;
        return Result.Success();
    }

    public Result UpdateState(GoalState state)
    {
        State = state;
        return Result.Success();
    }
}