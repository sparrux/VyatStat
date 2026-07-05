using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Search;

sealed class EventByOrganizerIdSpec : Specification<Event>
{
    public EventByOrganizerIdSpec(Guid organizerId)
    {
        Query.Where(e => e.Organizers.Any(x => x.Id == organizerId));
    }
}