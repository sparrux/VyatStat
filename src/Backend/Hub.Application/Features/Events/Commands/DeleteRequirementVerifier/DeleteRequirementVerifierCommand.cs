namespace Hub.Application.Features.Events.Commands.DeleteRequirementVerifier;

public sealed record DeleteRequirementVerifierCommand(
    Guid EventId,
    Guid RequirementId,
    Guid VerifierId
);
