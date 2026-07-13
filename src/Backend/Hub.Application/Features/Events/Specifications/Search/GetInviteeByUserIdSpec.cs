using Ardalis.Specification;
using Hub.Domain.Events.Invitees;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetInviteeByUserIdSpec : Specification<EventInvitee>
{
    public GetInviteeByUserIdSpec(Guid userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}