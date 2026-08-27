namespace Hub.Application.Features.Events.Commands.CreateRole;

public sealed record CreateRoleCommand(
    Guid EventId,
    CreateRoleRequest Request
);

public sealed record CreateRoleRequest(
    string Name,
    bool IsSealed
);
