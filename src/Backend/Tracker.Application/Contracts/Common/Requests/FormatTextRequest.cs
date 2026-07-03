using Tracker.Domain.Text;

namespace Tracker.Application.Contracts.Common.Requests;

public sealed record FormatTextRequest(
    string Text, 
    TextFormat Format
);