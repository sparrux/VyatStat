using Tracker.Domain.Text;

namespace Tracker.Application.Contracts.Common.Responses;

public sealed record FormatTextResponse(
    string Text,
    TextFormat Format
);