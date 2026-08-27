namespace Hub.Application.Features.Events.Commands.CreateParticipantRole;

public sealed record CreateParticipantRoleCommand(
    Guid EventId,
    Guid UserId,
    Guid RoleId
);
