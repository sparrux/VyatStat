using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Requirements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Invitees;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventInvitee : Auditable
{
    readonly List<EventRequirementCompletion> _requirementCompletions = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventInvitee() { }
    
    EventInvitee(User user)
    {
        User = user;
    }

    public User User { get; private set; }
    public Guid UserId { get; private set; }
    
    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    
    public EventInviteeRsvpStatus RsvpStatus { get; private set; }
    public EventAdmissionStatus AdmissionStatus { get; private set; }

    public IReadOnlyCollection<EventRequirementCompletion> RequirementCompletions =>
        _requirementCompletions;

    internal static Result<EventInvitee> Create(User user) => 
        Result.Success(new EventInvitee(user));

    internal Result UpdateRsvpStatus(EventInviteeRsvpStatus status)
    {
        RsvpStatus = status;
        return Result.Success();
    }
    
    internal Result UpdateAdmissionStatus(EventAdmissionStatus status)
    {
        AdmissionStatus = status;
        return Result.Success();
    }

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