using System.Diagnostics.CodeAnalysis;
using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Events;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Groups;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class GroupEvent : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    GroupEvent() { }

    GroupEvent(Group group, Event @event)
    {
        Group = group;
        Event = @event;
    }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }
    
    public static Result<GroupEvent> Create(Group group, Event @event)
    {
        if (ValidateGroup(group).Bind(() => ValidateEvent(@event)) is { IsSuccess: false } validation)
            return validation;
        
        return new GroupEvent(group, @event);
    }

    static Result ValidateGroup(Group? group)
    {
        return Result.FailIf(group is null, "Group cannot be null");
    }
    
    static Result ValidateEvent(Event? @event)
    {
        return Result.FailIf(@event is null, "Event cannot be null");
    }
}