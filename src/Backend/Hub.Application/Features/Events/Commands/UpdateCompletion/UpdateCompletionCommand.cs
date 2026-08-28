namespace Hub.Application.Features.Events.Commands.UpdateCompletion;

public sealed record UpdateCompletionCommand(
    Guid EventId,
    Guid RequirementId,
    
    Guid UserId,
    Guid? ActorId
);