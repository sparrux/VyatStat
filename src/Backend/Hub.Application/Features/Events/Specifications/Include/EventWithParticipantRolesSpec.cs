using Ardalis.Specification;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class EventWithParticipantRolesSpec : Specification<Event>
{
    public EventWithParticipantRolesSpec()
    {
        Query
            .Include(x => x.Roles)
                .ThenInclude(x => x.Participants)
            .Include(x => x.Participants)
                .ThenInclude(x => x.Roles)
                    .ThenInclude(x => x.Role);
    }
}
