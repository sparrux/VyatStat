using Ardalis.Specification;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain;

namespace Hub.Application.Features.Users.Specifications.Projection;

sealed class UserToDetailsSpec : Specification<User, UserDetailsResponse>
{
    public UserToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new UserDetailsResponse(
                x.Id,
                x.Nickname
            ));
    }
}