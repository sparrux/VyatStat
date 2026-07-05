using Ardalis.Specification;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.GroupMembers.Search;

sealed class MemberByUserIdSpec : Specification<GroupMember>
{
    public MemberByUserIdSpec(Guid userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}