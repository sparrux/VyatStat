using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Commands.CreateRequirement;

public sealed record CreateRequirementCommand(
    Guid EventId,
    CreateRequirementRequest Request
);

public sealed record CreateRequirementRequest(
    string Title,
    string? Description,
    RequirementAssignmentPolicy AssignmentPolicy
);
