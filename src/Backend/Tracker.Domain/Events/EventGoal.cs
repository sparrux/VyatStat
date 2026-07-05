using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Abstractions.Goals;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoal : Goal
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventGoal() { }
    
    EventGoal(string title, int currentValue, int targetValue) 
        : base(title, currentValue, targetValue)
    {
    }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public static Result<EventGoal> Create(string title, int current, int target)
    {
        if (ValidateTitle(title).Bind(() => ValidateValues(current, target)) 
            is { IsSuccess: false } validation)
            return validation;
        
        return Result.Ok(new EventGoal(title, current, target));
    }
}