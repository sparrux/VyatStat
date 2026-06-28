using FluentResults;

namespace Tracker.Domain.GroupEvents.Events;

public sealed class GroupEventTarget : Target
{
    GroupEventTarget() { }
    
    GroupEventTarget(string title, int currentValue, int targetValue) 
        : base(title, currentValue, targetValue)
    {
    }

    public Guid EventId { get; }
    public GroupEvent Event { get; }

    public static Result<GroupEventTarget> Create(string title, int current, int target)
    {
        if (ValidateTitle(title).Bind(() => ValidateValues(current, target)) 
            is { IsSuccess: false } validation)
            return validation;
        
        return Result.Ok(new GroupEventTarget(title, current, target));
    }
}