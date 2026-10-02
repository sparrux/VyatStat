using Hub.Domain.Common.DomainEvents;

namespace Hub.Domain.Events.Events;

public sealed class EventRequirementCreatedEvent : DomainEvent
{
    public EventRequirementCreatedEvent(Guid eventId, Guid eventRequirementId)
    {
        EventId = eventId;
        EventRequirementId = eventRequirementId;
    }

    public Guid EventId { get; }
    public Guid EventRequirementId { get; }
}