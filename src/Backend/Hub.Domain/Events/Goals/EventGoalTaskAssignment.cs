using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Goals;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventGoalTaskAssignment : Entity
{
    EventGoalTaskAssignment() { }

    EventGoalTaskAssignment(EventGoalTask task, EventParticipant assignment)
    {
        Task = task;
        ParticipantAssignment = assignment;
    }

    public Guid TaskId { get; private set; }
    public EventGoalTask Task { get; private set; }
    
    public Guid ParticipantAssignmentId { get; private set; }
    public EventParticipant ParticipantAssignment { get; private set; }

    internal static Result<EventGoalTaskAssignment> Create(
        EventGoalTask task, 
        EventParticipant assignment
    ) => new EventGoalTaskAssignment(task, assignment);
}