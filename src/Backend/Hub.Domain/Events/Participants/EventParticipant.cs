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
    readonly List<EventRequirementCompletion> _requirementCompletions = [];

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
    
    public IReadOnlyCollection<EventRequirementCompletion> RequirementCompletions =>
        _requirementCompletions;

    internal static Result<EventParticipant> Create(User user) => 
        Result.Success(new EventParticipant(user));

    internal Result<EventRequirementCompletion> AddCompletion(EventRequirement requirement)
    {
        var exists = RequirementCompletions
            .Any(c => c.RequirementId == requirement.Id);
        
        if (exists)
            return Result.Error("Event requirement completion already exists");
        
        var completion = EventRequirementCompletion
            .Create(this, requirement);
        
        if (!completion.IsSuccess)
            return completion;

        _requirementCompletions.Add(completion.Value);
        return completion;
    }
    
    internal Result RemoveCompletion(EventRequirementCompletion completion) => 
        !_requirementCompletions.Remove(completion) 
            ? Result.NotFound("Event requirement completion not found") 
            : Result.Success();
}