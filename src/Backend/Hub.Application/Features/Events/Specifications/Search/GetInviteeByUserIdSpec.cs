using Ardalis.Specification;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetInviteeByUserIdSpec : Specification<EventParticipant>
{
    public GetInviteeByUserIdSpec(Guid userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}