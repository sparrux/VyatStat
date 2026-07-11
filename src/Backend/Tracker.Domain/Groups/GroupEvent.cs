using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
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
    
    public static Result<GroupEvent> Create(Group group, Event @event) => 
        Result.Success(new GroupEvent(group, @event));
}