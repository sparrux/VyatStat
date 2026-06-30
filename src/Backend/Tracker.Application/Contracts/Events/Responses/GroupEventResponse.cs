using Tracker.Application.Contracts.Common.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Application.Contracts.Events.Responses;

public sealed record GroupEventResponse(
    Guid Id,
    Guid GroupId,
    string Title,
    GroupEventDescriptionResponse Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    GroupEventState State,
    LocationResponse? Location
);