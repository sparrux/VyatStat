using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Requirements.VerificationRules;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable CollectionNeverUpdated.Local
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

public abstract class EventRequirementVerifier : Auditable
{
    readonly List<EventRequirementVerification> _verifications = [];
    
    protected EventRequirementVerifier() { }

    protected EventRequirementVerifier(bool isRequired) => 
        IsRequired = isRequired;

    public bool IsRequired { get; private set; }

    public Guid RequirementId { get; private set; }
    public EventRequirement Requirement { get; private set; }

    public IReadOnlyCollection<EventRequirementVerification> Verifications => _verifications;
}

public sealed class EventRequirementRoleVerifier : EventRequirementVerifier
{
    EventRequirementRoleVerifier() { }
    
    EventRequirementRoleVerifier(
        bool isRequired, 
        EventRole verifier
    ) : base(isRequired) => Verifier = verifier;

    public Guid VerifierId { get; private set; }
    public EventRole Verifier { get; private set; }
    
    public static Result<EventRequirementRoleVerifier> Create(
        bool isRequired, 
        EventRole verifier
    ) => new EventRequirementRoleVerifier(isRequired, verifier);
}

public sealed class EventRequirementParticipantVerifier : EventRequirementVerifier
{
    EventRequirementParticipantVerifier() { }
    
    EventRequirementParticipantVerifier(
        bool isRequired, 
        EventParticipant verifier
    ) : base(isRequired) => Verifier = verifier;

    public Guid VerifierId { get; private set; }
    public EventParticipant Verifier { get; private set; }
    
    public static Result<EventRequirementParticipantVerifier> Create(
        bool isRequired, 
        EventParticipant verifier
    ) => new EventRequirementParticipantVerifier(isRequired, verifier);
}

public sealed class EventRequirementRuleVerifier : EventRequirementVerifier
{
    EventRequirementRuleVerifier() { }

    EventRequirementRuleVerifier(
        bool isRequired, 
        EventRequirementVerificationRule verifier
    ) : base(isRequired) => Verifier = verifier;

    public Guid VerifierId { get; private set; }
    public EventRequirementVerificationRule Verifier { get; private set; }
    
    public static Result<EventRequirementRuleVerifier> Create(
        bool isRequired,
        EventRequirementVerificationRule verifier
    ) => new EventRequirementRuleVerifier(isRequired, verifier);
}