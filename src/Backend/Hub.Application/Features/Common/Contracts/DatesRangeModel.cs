namespace Hub.Application.Features.Common.Contracts;

public record DatesRangeModel(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
);