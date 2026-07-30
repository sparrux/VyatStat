namespace Hub.Application.Features.Groups.Commands.AttachEvent;

public sealed record AttachEventCommand(
    Guid GroupId,
    Guid EventId
);