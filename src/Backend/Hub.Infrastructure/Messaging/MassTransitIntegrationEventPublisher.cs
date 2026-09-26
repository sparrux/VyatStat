using Hub.Application.Abstractions.Messaging;
using MassTransit;

namespace Hub.Infrastructure.Messaging;

sealed class MassTransitIntegrationEventPublisher(
    IPublishEndpoint publishEndpoint
) : IIntegrationEventPublisher
{
    public Task Publish(IIntegrationEvent message, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(message, message.GetType(), cancellationToken);
}
