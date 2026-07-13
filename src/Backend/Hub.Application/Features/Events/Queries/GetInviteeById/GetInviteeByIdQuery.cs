namespace Hub.Application.Features.Events.Queries.GetInviteeById;

public sealed record GetInviteeByIdQuery(
    Guid EventId,
    Guid InviteeUserId
);