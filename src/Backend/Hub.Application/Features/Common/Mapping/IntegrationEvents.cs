using Hub.Application.Abstractions.Messaging;
using Hub.Domain.Common.DomainEvents;

namespace Hub.Application.Features.Common.Mapping;

public static class IntegrationEvents
{
    private static readonly Func<IDomainEvent, IIntegrationEvent?>[] Mappers;
    
    static IntegrationEvents()
    {
        Mappers =
        [
            PaymentIntegrationEvents.From,
            EventIntegrationEvents.From,
        ];
    }
    
    public static IReadOnlyList<IIntegrationEvent> From(IEnumerable<IDomainEvent> domainEvents)
    {
        var messages = new List<IIntegrationEvent>();

        foreach (var domainEvent in domainEvents)
            foreach (var mapper in Mappers)
            {
                if (mapper.Invoke(domainEvent) is not { } message) 
                    continue;
                    
                messages.Add(message);
                break;
            }

        return messages;
    }
}
