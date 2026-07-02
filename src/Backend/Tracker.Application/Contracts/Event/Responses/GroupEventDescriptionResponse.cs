using Tracker.Domain.Text;

namespace Tracker.Application.Contracts.Event.Responses;

public sealed record GroupEventDescriptionResponse(
    string Text,
    TextFormat Format
);