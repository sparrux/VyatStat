namespace Hub.Application.Features.Groups.Queries.GetEvents;

public sealed record GetGroupEventsQuery(
    Guid GroupId,
    DateTimeOffset FromDate,
    DateTimeOffset ToDate
);