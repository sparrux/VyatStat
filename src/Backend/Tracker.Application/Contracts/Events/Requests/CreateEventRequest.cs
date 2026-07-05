using Tracker.Application.Contracts.Common.Requests;

namespace Tracker.Application.Contracts.Events.Requests;

public sealed record CreateEventRequest(
    string Title,
    EventDescriptionRequest Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    EventLocationRequest? Location
);