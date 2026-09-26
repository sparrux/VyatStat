namespace Hub.Domain.Common.DomainEvents;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent() : this(Guid.NewGuid())
    {
    }

    protected DomainEvent(Guid id)
    {
        Id = id;
        OccurredOn = DateTimeOffset.UtcNow;
    }
    
    public Guid Id { get; }
    public DateTimeOffset OccurredOn { get; }
}