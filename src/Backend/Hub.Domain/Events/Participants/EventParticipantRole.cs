using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Requirements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Participants;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class EventParticipantRole : Entity
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventParticipantRole() { }

    EventParticipantRole(EventRole role, EventParticipant participant)
    {
        Role = role;
        Participant = participant;
    }

    public Guid RoleId { get; private set; }
    public EventRole Role { get; private set; }

    public Guid ParticipantId { get; private set; }
    public EventParticipant Participant { get; private set; }

    internal static Result<EventParticipantRole> Create(
        EventRole role,
        EventParticipant participant
    ) => new EventParticipantRole(role, participant);
}