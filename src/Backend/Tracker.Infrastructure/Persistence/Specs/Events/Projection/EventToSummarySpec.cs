using Ardalis.Specification;
using Tracker.Application.Contracts.Events.Responses;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Projection;

sealed class EventToSummarySpec : Specification<Event, EventSummaryResponse>
{
    public EventToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventSummaryResponse(
                x.Id,
                x.Title,
                x.EndDate,
                x.StartDate,
                x.Invitees.Count,
                x.Organizers.Count));
    }
}