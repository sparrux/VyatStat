using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Commands.UpdateCompletion;

public sealed record UpdateCompletionCommand(
    Guid EventId,
    Guid UserId,
    Guid RequirementId,
    EventRequirementCompletionStatus NewStatus
);