using Ardalis.Specification;
using Hub.Application.Features.Groups.Contracts;
using Hub.Domain.Groups;

namespace Hub.Application.Features.Groups.Specifications.Projection;

sealed class GroupToSummarySpec : Specification<Group, GroupSummaryResponse>
{
    public GroupToSummarySpec()
    {
        Query.Select(x => new GroupSummaryResponse(
            x.Id,
            x.Name,
            x.Members.Count));
    }
}