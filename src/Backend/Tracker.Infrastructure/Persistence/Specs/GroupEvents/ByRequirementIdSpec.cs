using Ardalis.Specification;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class ByRequirementIdSpec : Specification<GroupEvent>
{
    public ByRequirementIdSpec(Guid reqId)
    {
        Query.Where(x => x.Requirements.Any(r => r.Id == reqId));
    }
}