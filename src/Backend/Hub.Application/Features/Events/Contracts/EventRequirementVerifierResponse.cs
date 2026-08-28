using System.Text.Json.Serialization;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Contracts;

public enum RequirementVerifierType
{
    Unknown,
    Role,
    Participant,
    Rule
}

public sealed record EventRequirementVerifierSummaryResponse(
    Guid Id,
    bool IsRequired,
    RequirementVerifierType Type
);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EventRequirementRoleVerifierResponse), nameof(RequirementVerifierType.Role))]
[JsonDerivedType(typeof(EventRequirementParticipantVerifierResponse), nameof(RequirementVerifierType.Participant))]
[JsonDerivedType(typeof(EventRequirementRuleVerifierResponse), nameof(RequirementVerifierType.Rule))]
public abstract record EventRequirementVerifierDetailsResponse(
    Guid Id,
    bool IsRequired
);

public sealed record EventRequirementRoleVerifierResponse(
    Guid Id,
    bool IsRequired,
    EventRoleSummaryResponse Role
) : EventRequirementVerifierDetailsResponse(Id, IsRequired);

public sealed record EventRequirementParticipantVerifierResponse(
    Guid Id,
    bool IsRequired,
    EventParticipantSummaryResponse Participant
) : EventRequirementVerifierDetailsResponse(Id, IsRequired);

public sealed record EventRequirementRuleVerifierResponse(
    Guid Id,
    bool IsRequired
) : EventRequirementVerifierDetailsResponse(Id, IsRequired);

public static class EventRequirementVerifierExtensions
{
    extension(EventRequirementVerifier verifier)
    {
        public RequirementVerifierType DetectResponseType()
        {
            return verifier switch
            {
                EventRequirementRoleVerifier => RequirementVerifierType.Role,
                EventRequirementParticipantVerifier => RequirementVerifierType.Participant,
                EventRequirementRuleVerifier => RequirementVerifierType.Rule,
                _ => RequirementVerifierType.Unknown
            };
        }

        public EventRequirementVerifierDetailsResponse ToResponse()
        {
            return verifier switch
            {
                EventRequirementRoleVerifier role => new EventRequirementRoleVerifierResponse(
                    role.Id,
                    role.IsRequired,
                    new EventRoleSummaryResponse(
                        role.Verifier.Id,
                        role.Verifier.Name,
                        role.Verifier.IsSealed)),
                EventRequirementParticipantVerifier participant => new EventRequirementParticipantVerifierResponse(
                    participant.Id,
                    participant.IsRequired,
                    new EventParticipantSummaryResponse(
                        new UserSummaryResponse(
                            participant.Verifier.User.Id,
                            participant.Verifier.User.Nickname))),
                EventRequirementRuleVerifier rule => new EventRequirementRuleVerifierResponse(
                    rule.Id,
                    rule.IsRequired),
                _ => throw new InvalidOperationException(
                    $"Unknown verifier type: {verifier.GetType().Name}")
            };
        }
    }
}