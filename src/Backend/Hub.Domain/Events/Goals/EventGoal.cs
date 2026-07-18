using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.AccessControl;
using Ardalis.Result;
using Hub.Domain.Concepts.Goals;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Goals;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoal : Goal
{
    readonly List<EventGoalTask> _tasks = new();

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventGoal() { }
    
    EventGoal(string name) : base(name) { }

    public IReadOnlyCollection<EventGoalTask> Tasks => _tasks;

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    internal static Result<EventGoal> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Event goal title cannot be null or whitespace"));
        
        return Result.Success(new EventGoal(title));
    }

    internal Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Event goal name cannot be null or whitespace"));

        Name = name;
        return Result.Success();
    }

    internal Result<EventGoalTask> CreateTask(string name)
    {
        var task = EventGoalTask.Create(name);
        if (!task.IsSuccess) return task;
        
        _tasks.Add(task.Value);
        return task;
    }
    
    internal Result RemoveTask(EventGoalTask task) =>
        _tasks.Remove(task)
            ? Result.Success()
            : Result.NotFound("Event Task not found");
}