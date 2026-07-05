using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Domain.Events;

namespace Tracker.Application.Contracts.Events.Responses;

public sealed record EventDetailsResponse(
    Guid Id,
    string Title,
    EventDescriptionResponse Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    EventState State,
    EventLocationResponse? Location,
    IReadOnlyCollection<EventOrganizerResponse> Organizers,
    IReadOnlyCollection<EventRequirementResponse> Requirements,
    IReadOnlyCollection<EventInviteeSummaryResponse> Invitees
);