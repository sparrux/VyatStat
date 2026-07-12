using Ardalis.Specification;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain;

namespace Hub.Application.Features.Users.Specifications.Projection;

sealed class UserToSummarySpec : Specification<User, UserSummaryResponse>
{
    public UserToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new UserSummaryResponse(x.Id, x.Nickname));
    }
}