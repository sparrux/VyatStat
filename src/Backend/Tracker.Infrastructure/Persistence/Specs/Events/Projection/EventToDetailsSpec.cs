using Ardalis.Specification;
using Tracker.Application.Contracts.Events.Responses;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Events.Projection;

sealed class EventToDetailsSpec : Specification<Event, EventDetailsResponse>
{
    public EventToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventDetailsResponse(
                x.Id,
                x.Title,
                new EventDescriptionResponse(x.Description.Text, x.Description.Format),
                x.EndDate,
                x.StartDate,
                x.State,
                x.Location != null ?
                    new EventLocationResponse(
                        x.Location.Id,
                        x.Location.Name,
                        x.Location.Latitude,
                        x.Location.Longitude)
                    : null,
                x.Organizers
                    .OrderBy(o => o.CreatedAt)
                    .Select(o => new EventOrganizerResponse(
                        o.Id,
                        new UserSummaryResponse(
                            o.User.Id,
                            o.User.Nickname,
                            o.User.CreatedAt))).ToList(),
                x.Requirements
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new EventRequirementResponse(
                        r.Id,
                        r.Title,
                        r.Description,
                        r.IsMandatory,
                        r.ConfirmationMode)).ToList(),
                x.Invitees
                    .OrderBy(o => o.CreatedAt)
                    .Select(i => new EventInviteeSummaryResponse(
                        i.Id,
                        new UserSummaryResponse(
                            i.User.Id,
                            i.User.Nickname,
                            i.User.CreatedAt))).ToList()
            ));
    }
}