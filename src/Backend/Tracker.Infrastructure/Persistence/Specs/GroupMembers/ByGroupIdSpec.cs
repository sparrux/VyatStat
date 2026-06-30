using Ardalis.Specification;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.GroupMembers;

sealed class ByGroupIdSpec : Specification<GroupMember>
{
    public ByGroupIdSpec(Guid groupId)
    {
        Query.Where(x => x.GroupId == groupId);
    }
}