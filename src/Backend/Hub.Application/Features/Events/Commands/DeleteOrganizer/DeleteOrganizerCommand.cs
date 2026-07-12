namespace Hub.Application.Features.Events.Commands.DeleteOrganizer;

public sealed record DeleteOrganizerCommand(
    Guid EventId,
    Guid UserId
);