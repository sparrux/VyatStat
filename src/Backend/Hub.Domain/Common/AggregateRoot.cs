using Hub.Domain.Common.DomainEvents;

namespace Hub.Domain.Common;

public abstract class AggregateRoot : Entity
{
    readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => 
        _domainEvents.Add(domainEvent);
}