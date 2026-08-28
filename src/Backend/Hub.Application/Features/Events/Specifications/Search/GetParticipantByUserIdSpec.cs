using Ardalis.Specification;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Specifications.Search;

sealed class GetParticipantByUserIdSpec : Specification<EventParticipant>
{
    public GetParticipantByUserIdSpec(Guid userId)
    {
        Query.Where(x => x.UserId == userId);
    }
}
