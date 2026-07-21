using Hub.Application.Features.Common.Contracts;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventDetailsResponse(
    Guid Id,
    string Title,
    RichTextModel? Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    EventState State,
    EventLocationResponse? Location,
    IReadOnlyCollection<EventRequirementSummaryResponse> Requirements,
    IReadOnlyCollection<EventInviteeSummaryResponse> Invitees
);