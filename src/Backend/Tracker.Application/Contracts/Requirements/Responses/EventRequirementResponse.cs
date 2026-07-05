using Tracker.Domain.Abstractions.Requirements;

namespace Tracker.Application.Contracts.Requirements.Responses;

public sealed record EventRequirementResponse(
    Guid Id,
    string Title,
    string? Description,
    bool IsMandatory,
    ConfirmationMode ConfirmationMode
);