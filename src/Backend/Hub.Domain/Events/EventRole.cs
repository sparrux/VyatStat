using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRole : Entity
{
    public const string Organizer = "Organizer";
    
    readonly List<EventParticipantRole> _participants = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRole() { }

    EventRole(string name, bool isSealed)
    {
        Name = name;
        IsSealed = isSealed;
    }

    public string Name { get; private set; }
    public bool IsSealed { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    
    public IReadOnlyCollection<EventParticipantRole> Participants => _participants;

    internal static Result<EventRole> Create(string name, bool isSealed)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Event Role name is required"));
        
        return new EventRole(name, isSealed);
    }

    internal Result UpdateName(string name)
    {
        if (IsSealed)
            return Result.Error("Role cannot be changed because sealed");

        if (string.IsNullOrWhiteSpace(name))
            return Result.Invalid(new ValidationError("Role name is required"));

        Name = name;
        return Result.Success();
    }
    
    internal Result<EventParticipantRole> AddParticipant(EventParticipant participant)
    {
        var participantRole = EventParticipantRole.Create(this, participant);
        if (!participantRole.IsSuccess) return participantRole;
        
        _participants.Add(participantRole.Value);
        return participantRole;
    }
    
    internal Result RemoveParticipant(EventParticipantRole participantRole)
    {
        if (IsSealed)
            return Result.Error("Participant Role cannot be removed because sealed");
        
        return !_participants.Remove(participantRole)
            ? Result.NotFound("Participant Role not found")
            : Result.Success();
    }
}