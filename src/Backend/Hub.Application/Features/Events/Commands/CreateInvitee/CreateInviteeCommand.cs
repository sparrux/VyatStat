namespace Hub.Application.Features.Events.Commands.CreateInvitee;

public sealed record CreateInviteeCommand(
    Guid EventId,
    Guid UserId
);