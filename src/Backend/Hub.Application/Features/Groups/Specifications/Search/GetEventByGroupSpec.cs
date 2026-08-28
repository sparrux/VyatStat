using Ardalis.Specification;
using Hub.Domain.Groups;

namespace Hub.Application.Features.Groups.Specifications.Search;

sealed class GetEventByGroupSpec : Specification<GroupEvent>
{
    public GetEventByGroupSpec(Guid groupId)
    {
        Query.Where(x => x.GroupId == groupId);
    }
}