using Tracker.Domain.Text;

namespace Tracker.Application.Contracts.Events.Responses;

public sealed record GroupEventDescriptionResponse(
    string Text,
    TextFormat Format
);