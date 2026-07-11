using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using Hub.Domain.Concepts.Goals;
using Hub.Domain.Validators;
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
        var titleValidation = new GoalTitleValidator().Validate(title);
        if (!titleValidation.IsValid)
            return Result.Invalid(titleValidation.AsErrors());
        
        return Result.Success(new EventGoal(title, state));
    }
}