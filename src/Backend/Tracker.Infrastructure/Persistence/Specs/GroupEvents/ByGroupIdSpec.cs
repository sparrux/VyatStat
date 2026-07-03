using Ardalis.Specification;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.GroupEvents;

sealed class ByGroupIdSpec : Specification<GroupEvent>
{
    public ByGroupIdSpec(Guid groupId)
    {
        Query.Where(x => x.GroupId == groupId);
    }
}