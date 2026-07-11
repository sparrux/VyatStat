using Ardalis.Result;
using FluentResults;
using Tracker.Domain.Common;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Abstractions.Goals;

public abstract class Goal : Auditable
{
    protected Goal() { }
    
    protected Goal(string title, int currentValue, int targetValue)
    {
        Title = title;
        
        if (currentValue > targetValue)
            throw new ArgumentException("Current must be less than target");
        
        CurrentValue = currentValue;
        TargetValue = targetValue;
    }
    
    public string Title { get; private set; }
    
    public int CurrentValue { get; private set; }

    public int TargetValue { get; private set; }

    public Result UpdateTitle(string title)
    {
        if (ValidateTitle(title) is { IsSuccess: false } validation)
            return validation;
        
        Title = title;
        
        return Result.Ok();
    }

    public Result SetCurrentValue(int currentValue)
    {
        if (ValidateCurrent(currentValue) is { IsSuccess: false } validation)
            return validation;
        
        CurrentValue = currentValue;
        
        return Result.Ok();
    }
    
    public Result SetTargetValue(int targetValue)
    {
        if (ValidateTarget(targetValue) is { IsSuccess: false } validation)
            return validation;
        
        TargetValue = targetValue;
        
        return Result.Ok();
    }
    
    protected static Result ValidateTitle(string title)
    {
        return Result.FailIf(string.IsNullOrWhiteSpace(title), "Title is required");
    }

    Result ValidateCurrent(int currentValue)
    {
        return Result.FailIf(currentValue > TargetValue, "Current must be less than target");
    }
    
    Result ValidateTarget(int targetValue)
    {
        return Result.FailIf(targetValue < CurrentValue, "Target must be larget than current");
    }
    
    protected static Result ValidateValues(int current, int target)
    {
        return Result.FailIf(current > target, "Current must be less than target");   
    }
}