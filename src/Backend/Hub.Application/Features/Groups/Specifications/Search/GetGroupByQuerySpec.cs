using Ardalis.Specification;
using Hub.Application.Features.Groups.Queries.Get;
using Hub.Domain.Groups;

namespace Hub.Application.Features.Groups.Specifications.Search;

sealed class GetGroupByQuerySpec : Specification<Group>
{
    public GetGroupByQuerySpec(GetGroupQuery query)
    {
        if (query.MemberUserId is { } memberUser)
            Query.Where(g => g.Members.Any(m => m.UserId == memberUser));
    }
}