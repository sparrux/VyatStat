using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Commands.UpdateState;

public sealed record UpdateStateCommand(
    Guid EventId,
    EventState NewState
);