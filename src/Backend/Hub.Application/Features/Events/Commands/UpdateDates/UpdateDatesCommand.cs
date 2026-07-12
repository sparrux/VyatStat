using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Commands.UpdateDates;

public sealed record UpdateDatesCommand(
    Guid EventId,
    UpdateEventDatesRequest Request
);

public sealed record UpdateEventDatesRequest(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
) : DatesRangeModel(StartDate, EndDate);