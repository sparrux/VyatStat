namespace Hub.Application.Features.Common.Contracts;

public abstract record DatesRangeModel(
    DateTimeOffset StartDate,
    DateTimeOffset EndDate
);