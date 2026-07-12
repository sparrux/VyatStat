using Ardalis.Specification;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Specifications.Projection;

sealed class EventToDetailsSpec : Specification<Event, EventDetailsResponse>
{
    public EventToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventDetailsResponse(
                x.Id,
                x.Title,
                x.Description != null ?
                    new RichTextModel(x.Description.Text, x.Description.Format)
                    : null,
                x.DatesRange.EndDate,
                x.DatesRange.StartDate,
                x.State,
                x.Location != null ?
                    new EventLocationResponse(
                        x.Location.Id,
                        x.Location.Name,
                        x.Location.Coordinates.Latitude,
                        x.Location.Coordinates.Longitude)
                    : null,
                x.Organizers
                    .OrderBy(o => o.CreatedAt)
                    .Select(o => new EventOrganizerResponse(
                        o.Id,
                        new UserSummaryResponse(
                            o.User.Id,
                            o.User.Nickname))).ToList(),
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
                            i.User.Nickname))).ToList()
            ));
    }
}