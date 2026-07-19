using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRequirementCompletion : Auditable
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirementCompletion() { }
    
    EventRequirementCompletion(
        EventParticipant participant, EventRequirement requirement)
    {
        Participant = participant;
        Requirement = requirement;
    }
    
    public Guid InviteeId { get; private set; }
    public EventParticipant Participant { get; private set; }
    
    public Guid RequirementId { get; private set; }
    public EventRequirement Requirement { get; private set; }
    
    public RequirementVerificationStatus VerificationStatus { get; private set; }
    
    internal static Result<EventRequirementCompletion> Create(
        EventParticipant eventParticipant, EventRequirement requirement) =>
        Result.Success(new EventRequirementCompletion(eventParticipant, requirement));
    
    Result UpdateVerification(RequirementVerificationStatus verification)
    {
        VerificationStatus = verification;
        return Result.Success();
    }
    
    internal Result PendingVerification() => 
        UpdateVerification(RequirementVerificationStatus.PendingVerification);
    
    internal Result Verify() => 
        UpdateVerification(RequirementVerificationStatus.Verified);

    internal Result Reject() => 
        UpdateVerification(RequirementVerificationStatus.Rejected);
}