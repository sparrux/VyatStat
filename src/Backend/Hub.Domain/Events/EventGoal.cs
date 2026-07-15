using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Goals;
using Hub.Domain.ValueObjects;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoal : Goal
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventGoal() { }
    
    EventGoal(string title, int currentValue, int targetValue) 
        : base(title, currentValue, targetValue) { }

    EventGoal(string title, GoalState state) : base(title, state) { }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    internal static Result<EventGoal> Create(string title, GoalState state)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event goal title cannot be null or whitespace"));
        
        return Result.Success(new EventGoal(title, state));
    }
}