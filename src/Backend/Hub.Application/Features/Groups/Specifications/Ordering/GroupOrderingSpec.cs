using Ardalis.Specification;
using Hub.Domain.Groups;

namespace Hub.Application.Features.Groups.Specifications.Ordering;

sealed class GroupOrderingSpec : Specification<Group>
{
    public GroupOrderingSpec()
    {
        Query
            .OrderByDescending(g => g.CreatedAt)
            .ThenByDescending(x => x.Members.Count);
    }
}