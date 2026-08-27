using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Concepts.Requirements;
using Hub.Domain.Events.Participants;
using Hub.Domain.Events.Requirements.VerificationRules;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Hub.Domain.Events.Requirements;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
[SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class EventRequirement : Requirement
{
    readonly List<EventRequirementVerifier> _verifiers = [];
    readonly List<EventRequirementAssignment> _assignments = [];

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    EventRequirement() { }

    EventRequirement(
        string title,
        string? description,
        RequirementAssignmentPolicy assignmentPolicy
    ) : base(title, description)
    {
        AssignmentPolicy = assignmentPolicy;
    }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; }
    
    public RequirementAssignmentPolicy AssignmentPolicy { get; private set; }

    public IReadOnlyCollection<EventRequirementVerifier> Verifiers => _verifiers;
    public IReadOnlyCollection<EventRequirementAssignment> Assignments => _assignments;
    
    internal static Result<EventRequirement> Create(
        string title, string? description, RequirementAssignmentPolicy assignmentPolicy)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Invalid(new ValidationError("Title cannot be null or whitespace"));
        
        return new EventRequirement(title, description, assignmentPolicy);
    }

    internal Result<EventRequirementRoleVerifier> AddRoleVerifier(EventRole role, bool isRequired)
    {
        if (Verifiers.OfType<EventRequirementRoleVerifier>()
            .Any(x => x.Verifier == role))
            return Result.Error("Event Role already added to verifiers");

        var verifier = EventRequirementRoleVerifier.Create(isRequired, role);
        if (!verifier.IsSuccess) return verifier;
        
        _verifiers.Add(verifier.Value);
        return verifier;
    }
    
    internal Result<EventRequirementParticipantVerifier> AddParticipantVerifier(
        EventParticipant participant, bool isRequired)
    {
        if (Verifiers.OfType<EventRequirementParticipantVerifier>()
            .Any(x => x.Verifier == participant))
            return Result.Error("Participant already added to verifiers");

        var verifier = EventRequirementParticipantVerifier.Create(isRequired, participant);
        if (!verifier.IsSuccess) return verifier;
        
        _verifiers.Add(verifier.Value);
        return verifier;
    }

    public Result<EventRequirementRuleVerifier> AddRuleVerifier(
        EventRequirementVerificationRule rule, bool isRequired)
    {
        if (Verifiers.OfType<EventRequirementRuleVerifier>()
            .Any(x => x.Verifier == rule))
            return Result.Error("Rule already added to verifiers");

        var verifier = EventRequirementRuleVerifier.Create(isRequired, rule);
        if (!verifier.IsSuccess) return verifier;
        
        _verifiers.Add(verifier.Value);
        return verifier;
    }
}