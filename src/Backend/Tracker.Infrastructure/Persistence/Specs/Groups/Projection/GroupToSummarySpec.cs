using Ardalis.Specification;
using Tracker.Application.Contracts.Groups.Responses;
using Tracker.Domain.Groups;

namespace Tracker.Infrastructure.Persistence.Specs.Groups.Projection;

sealed class GroupToSummarySpec : Specification<Group, GroupSummaryResponse>
{
    public GroupToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(g => new GroupSummaryResponse(
                g.Id,
                g.Name,
                g.Members.Count));
    }
}