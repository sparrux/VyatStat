using Ardalis.Specification;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.GroupMembers;

sealed class ByUserIdSpec : Specification<GroupMember>
{
    public ByUserIdSpec(Guid userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}