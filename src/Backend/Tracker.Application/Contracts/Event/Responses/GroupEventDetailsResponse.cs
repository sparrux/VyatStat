using Tracker.Application.Contracts.Common.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventDetailsResponse(
    Guid Id,
    Guid GroupId,
    string Title,
    FormatTextResponse Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    GroupEventState State,
    LocationResponse? Location
);