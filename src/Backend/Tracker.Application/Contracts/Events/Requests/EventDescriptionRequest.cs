using Tracker.Domain.Abstractions.Text;

namespace Tracker.Application.Contracts.Events.Requests;

public sealed record EventDescriptionRequest(
    string Text, 
    TextFormat Format
);