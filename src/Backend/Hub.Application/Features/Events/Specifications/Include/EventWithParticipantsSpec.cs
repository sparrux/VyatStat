using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithParticipantsSpec : Specification<Event>
{
    public EventWithParticipantsSpec()
    {
        Query.Include(x => x.Participants);
    }
}