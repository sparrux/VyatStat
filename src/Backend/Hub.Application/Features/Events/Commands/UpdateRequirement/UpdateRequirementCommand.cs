using Hub.Domain.Concepts.Requirements;

namespace Hub.Application.Features.Events.Commands.UpdateRequirement;

public sealed record UpdateRequirementCommand(
    Guid EventId,
    Guid RequirementId,
    UpdateRequirementRequest Request
);

public sealed record UpdateRequirementRequest(
    string Title,
    string? Description,
    bool IsMandatory,
    ConfirmationMode ConfirmationMode
);