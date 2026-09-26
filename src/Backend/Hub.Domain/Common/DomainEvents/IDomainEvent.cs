namespace Hub.Domain.Common.DomainEvents;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTimeOffset OccurredOn { get; }
}