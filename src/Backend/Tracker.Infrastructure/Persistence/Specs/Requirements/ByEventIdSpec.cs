using Ardalis.Specification;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.Specs.Requirements;

sealed class ByEventIdSpec : Specification<GroupEventRequirement>
{
    public ByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}