namespace Hub.Application.Features.Events.Commands.DeleteParticipantRole;

public sealed record DeleteParticipantRoleCommand(
    Guid EventId,
    Guid UserId,
    Guid RoleId
);
