using Ardalis.Specification;
using Hub.Application.Features.Users.Queries.Get;
using Hub.Domain;

namespace Hub.Application.Features.Users.Specifications.Search;

sealed class GetUserByQuerySpec : Specification<User>
{
    public GetUserByQuerySpec(GetUserQuery query)
    {
    }
}