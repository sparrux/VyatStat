namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier;

public abstract record CreateRequirementVerifierRequest(
    bool IsRequired
);

public sealed record CreateRequirementRoleVerifierRequest(
    Guid RoleId,
    bool IsRequired
) : CreateRequirementVerifierRequest(IsRequired);

public sealed record CreateRequirementParticipantVerifierRequest(
    Guid ParticipantUserId,
    bool IsRequired
) : CreateRequirementVerifierRequest(IsRequired);

public sealed record CreateRequirementRuleVerifierRequest(
    bool IsRequired
) : CreateRequirementVerifierRequest(IsRequired);

public sealed record CreateRequirementVerifierCommand(
    Guid EventId,
    Guid RequirementId,
    CreateRequirementVerifierRequest Request
);