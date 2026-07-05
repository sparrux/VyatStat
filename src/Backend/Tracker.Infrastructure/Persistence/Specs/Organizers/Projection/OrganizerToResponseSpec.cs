using Ardalis.Specification;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Organizers.Projection;

sealed class OrganizerToResponseSpec : Specification<EventOrganizer, EventOrganizerResponse>
{
    public OrganizerToResponseSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventOrganizerResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id,
                    x.User.Nickname,
                    x.User.CreatedAt)));
    }
}