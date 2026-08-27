namespace Hub.Application.Features.Events.Commands.DeleteRole;

public sealed record DeleteRoleCommand(
    Guid EventId,
    Guid RoleId
);
