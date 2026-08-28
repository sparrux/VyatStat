namespace Hub.Application.Features.Events.Queries.GetById;

public sealed record GetEventByIdQuery(
    Guid EventId
);