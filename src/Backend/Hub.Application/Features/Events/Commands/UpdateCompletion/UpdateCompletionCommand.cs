namespace Hub.Application.Features.Events.Commands.UpdateCompletion;

public sealed record UpdateCompletionCommand(
    Guid EventId,
    Guid UserId,
    Guid RequirementId,
    Guid? ActorId
);