using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.GroupEvents.Requests;

public sealed record CreateGroupEventRequest(
    string Title,
    FormatTextRequest Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    LocationRequest? Location
);