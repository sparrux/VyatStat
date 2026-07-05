using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Search;

sealed class EventByGroupIdSpec : Specification<Event>
{
    public EventByGroupIdSpec(Guid groupId)
    {
        Query.Where(e => e.GroupEvents.Any(x => x.GroupId == groupId));
    }
}