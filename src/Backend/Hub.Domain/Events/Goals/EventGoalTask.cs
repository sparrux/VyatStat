using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;

// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Goals;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoalTask : Entity
{
    readonly List<EventGoalTaskAssignment> _assignments = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventGoalTask() { }
    
    EventGoalTask(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
    
    public Guid GoalId { get; private set; }
    public EventGoal Goal { get; private set; }

    public IReadOnlyCollection<EventGoalTaskAssignment> Assignments => _assignments;

    internal static Result<EventGoalTask> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Goal Task Name cannot be null or whitespace"));

        return new EventGoalTask(name);
    }

    internal Result<EventGoalTaskAssignment> Assign(EventGoalTask task, EventParticipant assignment)
    {
        var assign = EventGoalTaskAssignment.Create(task, assignment);
        if (!assign.IsSuccess) return assign;
        
        _assignments.Add(assign.Value);
        return assign;
    }
    
    internal Result<EventGoalTaskAssignment> RemoveAssignment(EventGoalTaskAssignment assignment) =>
        _assignments.Remove(assignment) 
            ? Result.Success() 
            : Result.NotFound("Goal Task Assignment not found");

    internal Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Goal Task Name cannot be null or whitespace"));

        Name = name;
        return Result.Success();
    }
}