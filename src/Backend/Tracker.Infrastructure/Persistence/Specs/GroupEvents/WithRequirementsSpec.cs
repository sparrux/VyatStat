using Ardalis.Specification;
using Tracker.Domain.GroupEvents;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class WithRequirementsSpec : Specification<GroupEvent>
{
    public WithRequirementsSpec()
    {
        Query.Include(x => x.Requirements);
    }
}