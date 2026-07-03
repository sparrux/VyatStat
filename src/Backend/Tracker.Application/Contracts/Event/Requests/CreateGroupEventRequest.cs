using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.Event.Requests;

public sealed record CreateGroupEventRequest(
    string Title,
    FormatTextRequest Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    LocationRequest? Location
);