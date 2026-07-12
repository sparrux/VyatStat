namespace Hub.Application.Features.Events.Commands.CreateOrganizer;

public sealed record CreateOrganizerCommand(
    Guid EventId,
    Guid UserId
);