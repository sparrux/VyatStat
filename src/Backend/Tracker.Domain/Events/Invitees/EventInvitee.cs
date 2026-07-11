using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using FluentResults;
using Tracker.Domain.Common;
using Tracker.Domain.Events.Requirements;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Tracker.Domain.Events.Invitees;

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

    public static Result<EventInvitee> Create(User user)
    {
        if (ValidateUser(user) is { IsSuccess: false } validation)
            return validation;

        return Result.Ok(new EventInvitee(user));
    }
    
    public Result UpdateRsvpStatus(EventInviteeRsvpStatus status)
    {
        RsvpStatus = status;
        return Result.Ok();
    }
    
    public Result UpdateAdmissionStatus(EventAdmissionStatus status)
    {
        AdmissionStatus = status;
        return Result.Ok();
    }

    public Result<EventRequirementCompletion> AddCompletion(EventRequirement requirement)
    {
        var completion = EventRequirementCompletion
            .Create(this, requirement);
        
        if (completion.IsFailed)
            return completion;

        _requirementCompletions.Add(completion.Value);

        return completion;
    }
    
    public Result RemoveCompletion(EventRequirementCompletion completion)
    {
        if (!_requirementCompletions.Remove(completion))
            return Result.Fail("Requirement completion not found");
        
        return Result.Ok();
    }
    
    static Result ValidateUser(User? user)
    {
        return Result.FailIf(user is null, "User is required");
    }
}