namespace Hub.Application.Features.Events.Commands.CreateParticipant;

public sealed record CreateParticipantCommand(
    Guid EventId,
    Guid UserId
);
