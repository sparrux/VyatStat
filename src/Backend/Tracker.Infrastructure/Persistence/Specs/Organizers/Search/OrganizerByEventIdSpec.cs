using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Organizers.Search;

sealed class OrganizerByEventIdSpec : Specification<EventOrganizer>
{
    public OrganizerByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}