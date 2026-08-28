namespace Hub.Domain.Common.DomainEvents;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid())
    {
        EventId = Guid.NewGuid();
    }

    protected DomainEvent(Guid eventId)
    {
        EventId = eventId;
    }
    
    public Guid EventId { get; }
    public DateTimeOffset OccurredOn { get; }
}