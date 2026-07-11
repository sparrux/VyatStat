using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Invitees;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRequirementCompletion : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirementCompletion() { }
    
    EventRequirementCompletion(
        EventInvitee invitee, EventRequirement requirement)
    {
        Invitee = invitee;
        Requirement = requirement;
    }
    
    public Guid InviteeId { get; private set; }
    public EventInvitee Invitee { get; private set; }
    
    public Guid RequirementId { get; private set; }
    public EventRequirement Requirement { get; private set; }
    
    public EventRequirementCompletionStatus CompletionStatus { get; private set; }
    
    internal static Result<EventRequirementCompletion> Create(
        EventInvitee eventInvitee, EventRequirement requirement) =>
        Result.Success(new EventRequirementCompletion(eventInvitee, requirement));

    public Result<EventRequirementCompletion> UpdateCompletionStatus(
        EventRequirementCompletionStatus completionStatus)
    {
        CompletionStatus = completionStatus;
        return Result.Success();
    }
}