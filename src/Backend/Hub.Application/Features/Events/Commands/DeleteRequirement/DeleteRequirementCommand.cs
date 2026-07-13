namespace Hub.Application.Features.Events.Commands.DeleteRequirement;

public sealed record DeleteRequirementCommand(
    Guid EventId,
    Guid RequirementId
);