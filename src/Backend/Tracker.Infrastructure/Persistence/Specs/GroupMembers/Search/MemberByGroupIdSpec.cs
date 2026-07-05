using Ardalis.Specification;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.GroupMembers.Search;

sealed class MemberByGroupIdSpec : Specification<GroupMember>
{
    public MemberByGroupIdSpec(Guid groupId)
    {
        Query.Where(x => x.GroupId == groupId);
    }
}