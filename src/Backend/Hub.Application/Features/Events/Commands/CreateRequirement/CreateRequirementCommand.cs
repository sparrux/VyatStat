using Hub.Domain.Concepts.Requirements;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

public sealed record CreateRequirementCommand(
    Guid EventId,
    CreateRequirementRequest Request
);

public sealed record CreateRequirementRequest(
    string Title,
    string? Description,
    bool IsMandatory,
    RequirementVerificationMode VerificationMode
);