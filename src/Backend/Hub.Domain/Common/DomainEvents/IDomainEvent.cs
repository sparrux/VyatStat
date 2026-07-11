namespace Hub.Domain.Common.DomainEvents;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredOn { get; }
}