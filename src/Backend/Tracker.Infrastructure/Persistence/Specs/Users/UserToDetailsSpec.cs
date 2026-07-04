using Ardalis.Specification;
using Tracker.Application.Contracts.Groups.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain;

namespace Tracker.Infrastructure.Persistence.Specs.Users;

sealed class UserToDetailsSpec : Specification<User, UserDetailsResponse>
{
    public UserToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(user => new UserDetailsResponse(
                user.Id, 
                user.Nickname, 
                user.Memberships
                    .Select(member => member.Group)
                    .Select(group => new GroupSummaryResponse(group.Id, group.Name, group.Members.Count))
                    .ToList(),
                user.CreatedAt));
    }
}