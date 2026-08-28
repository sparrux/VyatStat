namespace Hub.Application.Features.Events.Commands.UpdateTitle;

public sealed record UpdateTitleCommand(
    Guid EventId,
    UpdateTitleRequest Request
);

public sealed record UpdateTitleRequest(
    string NewTitle
);