using Ardalis.Specification;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class WithLocationSpec : Specification<GroupEvent>
{
    public WithLocationSpec()
    {
        Query
            .Include(x => x.Location)
            .ThenInclude(x => x.Location);
    }
}