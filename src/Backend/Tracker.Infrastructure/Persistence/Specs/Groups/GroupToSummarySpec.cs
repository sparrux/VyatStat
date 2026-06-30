using Ardalis.Specification;
using Tracker.Application.Contracts.Group.Responses;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.Groups;

sealed class GroupToSummarySpec : Specification<Group, GroupSummaryResponse>
{
    public GroupToSummarySpec()
    {
        Query.Select(g => new GroupSummaryResponse(
            g.Id,
            g.Name,
            g.Members.Count));
    }
}