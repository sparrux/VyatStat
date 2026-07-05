using Tracker.Domain.Abstractions;
using Tracker.Domain.Abstractions.Requirements;

namespace Tracker.Application.Contracts.Requirements.Requests;

public sealed record CreateEventRequirementRequest(
    string Title, 
    string? Description, 
    bool IsMandatory,
    ConfirmationMode ConfirmationMode
);