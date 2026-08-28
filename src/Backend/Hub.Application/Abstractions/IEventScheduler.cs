using Hub.Domain.Events;

namespace Hub.Application.Abstractions;

public interface IEventScheduler
{
    Task ScheduleAsync(Event evt, CancellationToken cancellationToken);
    Task DeleteAsync(Event evt, CancellationToken cancellationToken);
}
