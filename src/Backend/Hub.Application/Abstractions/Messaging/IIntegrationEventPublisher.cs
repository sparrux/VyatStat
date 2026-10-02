namespace Hub.Application.Abstractions.Messaging;

public interface IIntegrationEventPublisher
{
    Task Publish(IIntegrationEvent message, CancellationToken cancellationToken);
}
