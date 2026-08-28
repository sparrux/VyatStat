using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public abstract class EventRequirementVerification : Entity
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    protected EventRequirementVerification() { }

    protected EventRequirementVerification(
        EventRequirementVerifier verifier, 
        EventRequirementAssignment assignment)
    {
        Verifier = verifier;
        RequirementAssignment = assignment;
    }
    
    public Guid VerifierId { get; private set; }
    public EventRequirementVerifier Verifier { get; private set; }

    public Guid RequirementAssignmentId { get; private set; }
    public EventRequirementAssignment RequirementAssignment { get; private set; }
}

public sealed class EventRequirementRoleVerification : EventRequirementVerification
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirementRoleVerification() { }
    
    EventRequirementRoleVerification(
        EventRequirementVerifier verifier, 
        EventRequirementAssignment assignment,
        EventParticipantRole verifiedBy
    ) : base(verifier, assignment)
    {
        VerifiedBy = verifiedBy;
    }

    public Guid VerifiedById { get; private set; }
    public EventParticipantRole VerifiedBy { get; private set; }
    
    public static Result<EventRequirementVerification> Create(
        EventRequirementVerifier verifier, 
        EventRequirementAssignment assignment,
        EventParticipantRole verifiedBy
    ) => new EventRequirementRoleVerification(verifier, assignment, verifiedBy);
}

public sealed class EventRequirementParticipantVerification : EventRequirementVerification
{
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirementParticipantVerification() { }
    
    EventRequirementParticipantVerification(
        EventRequirementVerifier verifier, 
        EventRequirementAssignment assignment,
        EventParticipant verifiedBy
    ) : base(verifier, assignment)
    {
        VerifiedBy = verifiedBy;
    }

    public Guid VerifiedById { get; private set; }
    public EventParticipant VerifiedBy { get; private set; }
    
    public static Result<EventRequirementParticipantVerification> Create(
        EventRequirementVerifier verifier, 
        EventRequirementAssignment assignment,
        EventParticipant verifiedBy
    ) => new EventRequirementParticipantVerification(verifier, assignment, verifiedBy);
}