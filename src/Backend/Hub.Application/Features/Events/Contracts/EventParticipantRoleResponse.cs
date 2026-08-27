namespace Hub.Application.Features.Events.Contracts;

public sealed record EventParticipantRoleResponse(
    Guid Id,
    EventRoleSummaryResponse Role
);
