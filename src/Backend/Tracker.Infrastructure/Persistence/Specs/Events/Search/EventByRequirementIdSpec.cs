using Ardalis.Specification;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Search;

sealed class EventByRequirementIdSpec : Specification<Event>
{
    public EventByRequirementIdSpec(Guid reqId)
    {
        Query.Where(x => x.Requirements.Any(r => r.Id == reqId));
    }
}