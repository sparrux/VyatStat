using Ardalis.Specification;
using Tracker.Application.Contracts.GroupMembers.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.GroupMembers;

sealed class GroupMemberToSummarySpec : Specification<GroupMember, GroupMemberSummaryResponse>
{
    public GroupMemberToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(member => new GroupMemberSummaryResponse(
                new UserSummaryResponse(member.User.Id, member.User.Nickname, member.User.CreatedAt),
                member.GroupId));
    }
}