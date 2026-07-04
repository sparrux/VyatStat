using Ardalis.Specification;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain;

namespace Tracker.Infrastructure.Persistence.Specs.Users;

sealed class UserToSummarySpec : Specification<User, UserSummaryResponse>
{
    public UserToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new UserSummaryResponse(x.Id, x.Nickname, x.CreatedAt));
    }
}