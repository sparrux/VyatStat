using Hub.Application.Abstractions.Messaging;
using Hub.Application.Features.Events.Messages;
using Hub.Domain.Common.DomainEvents;
using Hub.Domain.Events.Events;

namespace Hub.Application.Features.Common.Mapping;

static class EventIntegrationEvents
{
    public static IIntegrationEvent? From(this IDomainEvent domainEvent) => 
        domainEvent switch
        {
            EventRequirementCreatedEvent requirementCreated => new EventRequirementCreated(
                requirementCreated.EventId,
                requirementCreated.EventRequirementId
            ),
            
            _ => null
        };
}