using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Commands.Create;

public sealed record CreateEventCommand(
    Guid OrganizerUserId,
    CreateEventRequest Request
);

public sealed record CreateEventRequest(
    string Title,
    CreateEventDatesRequest Dates
);

public sealed record CreateEventDatesRequest(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
) : DatesRangeModel(StartDate, EndDate);