using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Include;

sealed class EventWithLocationSpec : Specification<Event>
{
    public EventWithLocationSpec()
    {
        Query.Include(x => x.Location);
    }
}