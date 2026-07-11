using Hub.Domain.Concepts.Requirements;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventRequirementResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsMandatory,
    ConfirmationMode ConfirmationMode
);