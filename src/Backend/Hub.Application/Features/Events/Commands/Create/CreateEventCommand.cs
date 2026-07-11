using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Commands.Create;

public sealed record CreateEventCommand(
    string Title,
    CreateEventDatesCommand Dates
);

public sealed record CreateEventDatesCommand(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
) : DatesRangeModel(StartDate, EndDate);