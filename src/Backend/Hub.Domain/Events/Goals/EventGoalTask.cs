using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Goals;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoalTask : Entity
{
    EventGoalTask(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
    
    public Guid GoalId { get; private set; }
    public EventGoal Goal { get; private set; }

    internal static Result<EventGoalTask> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Goal Task Name cannot be null or whitespace"));

        return new EventGoalTask(name);
    }
    
    internal Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Goal Task Name cannot be null or whitespace"));

        Name = name;
        return Result.Success();
    }
}