using Tracker.Domain.Abstractions.Text;

namespace Tracker.Application.Contracts.Events.Responses;

public sealed record EventDescriptionResponse(
    string Text,
    TextFormat Format
);