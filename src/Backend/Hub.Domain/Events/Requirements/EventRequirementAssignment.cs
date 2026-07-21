using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
// ReSharper disable CollectionNeverUpdated.Local

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRequirementAssignment : Auditable
{
    readonly List<EventRequirementVerification> _verifications = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirementAssignment() { }
    
    EventRequirementAssignment(EventParticipant assign, EventRequirement requirement)
    {
        AssignParticipant = assign;
        Requirement = requirement;
    }
    
    public Guid AssignParticipantId { get; private set; }
    public EventParticipant AssignParticipant { get; private set; }
    
    public Guid RequirementId { get; private set; }
    public EventRequirement Requirement { get; private set; }

    public IReadOnlyCollection<EventRequirementVerification> Verifications => _verifications;
    
    internal static Result<EventRequirementAssignment> Create(
        EventParticipant assign, EventRequirement requirement) =>
        Result.Success(new EventRequirementAssignment(assign, requirement));

    internal Result<EventRequirementVerification> VerifyByRole(
        EventRequirementRoleVerifier verifier,
        EventParticipantRole verifiedBy)
    {
        var verification = EventRequirementRoleVerification.Create(verifier, this, verifiedBy);
        if (!verification.IsSuccess) return verification;
        
        _verifications.Add(verification.Value);
        return verification;
    }
}