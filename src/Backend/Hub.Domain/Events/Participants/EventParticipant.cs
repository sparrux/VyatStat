using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Requirements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Participants;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class EventParticipant : Auditable
{
    readonly List<EventParticipantRole> _roles = [];
    readonly List<EventRequirementAssignment> _requirements = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventParticipant() { }
    
    EventParticipant(User user)
    {
        User = user;
    }

    public User User { get; private set; }
    public Guid UserId { get; private set; }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }

    public IReadOnlyCollection<EventParticipantRole> Roles => _roles;
    public IReadOnlyCollection<EventRequirementAssignment> Requirements => _requirements;

    internal static Result<EventParticipant> Create(User user) => 
        Result.Success(new EventParticipant(user));

    internal Result<EventRequirementAssignment> Assign(EventRequirement requirement)
    {
        var exists = Requirements
            .Any(c => c.Requirement == requirement);
        
        if (exists)
            return Result.Error("Event requirement completion already exists");
        
        var completion = EventRequirementAssignment
            .Create(this, requirement);
        
        if (!completion.IsSuccess)
            return completion;

        _requirements.Add(completion.Value);
        return completion;
    }
    
    internal Result RemoveAssignment(EventRequirementAssignment completion) => 
        !_requirements.Remove(completion) 
            ? Result.NotFound("Event requirement completion not found") 
            : Result.Success();
}