using Hub.Application.Abstractions.Messaging;

namespace Hub.Application.Features.Events.Messages;

public sealed record EventRequirementCreated(
    Guid EventId,
    Guid EventRequirementId
) : IIntegrationEvent;