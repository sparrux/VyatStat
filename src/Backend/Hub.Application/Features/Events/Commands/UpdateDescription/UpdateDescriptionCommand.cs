using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Commands.UpdateDescription;

public sealed record UpdateDescriptionCommand(
    Guid EventId,
    UpdateDescriptionRequest Request
);

public sealed record UpdateDescriptionRequest(
    RichTextModel NewDescription
);