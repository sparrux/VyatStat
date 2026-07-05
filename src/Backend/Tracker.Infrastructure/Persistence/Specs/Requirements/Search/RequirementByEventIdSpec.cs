using Ardalis.Specification;
using Tracker.Domain.Events.Requirements;

namespace Tracker.Infrastructure.Persistence.Specs.Requirements.Search;

sealed class RequirementByEventIdSpec : Specification<EventRequirement>
{
    public RequirementByEventIdSpec(Guid eventId)
    {
        Query.Where(x => x.EventId == eventId);
    }
}