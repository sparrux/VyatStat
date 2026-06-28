using FluentResults;
using Tracker.Domain.Common;

namespace Tracker.Domain;

public abstract class Target : Auditable
{
    protected Target() { }
    
    protected Target(string title, int currentValue, int targetValue)
    {
        Title = title;
        
        if (currentValue > targetValue)
            throw new ArgumentException("Current must be less than target");
        
        CurrentValue = currentValue;
        TargetValue = targetValue;
    }
    
    /// <summary>
    /// Title of target.
    /// </summary>
    public string Title { get; private set; }
    
    /// <summary>
    /// Indicates whether the target has been fully achieved.
    /// </summary>
    public bool IsAchieved { get; private set; }

    /// <summary>
    /// Current value used to evaluate progress.
    /// </summary>
    public int CurrentValue { get; private set; }

    /// <summary>
    /// Target value to be reached.
    /// </summary>
    public int TargetValue { get; private set; }

    public Result Achieve()
    {
        return SetCurrentValue(CurrentValue < TargetValue ? TargetValue : CurrentValue);
    }
    
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
        UpdateAchieved();
        
        return Result.Ok();
    }
    
    public Result SetTargetValue(int targetValue)
    {
        if (ValidateTarget(targetValue) is { IsSuccess: false } validation)
            return validation;
        
        TargetValue = targetValue;
        UpdateAchieved();
        
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

    void UpdateAchieved()
    {
        if (CurrentValue >= TargetValue)
            IsAchieved = true;
    }
}