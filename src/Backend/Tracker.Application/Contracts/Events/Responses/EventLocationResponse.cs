namespace Tracker.Application.Contracts.Events.Responses;

public sealed record EventLocationResponse(
    Guid Id,
    string? Name,
    double Latitude,
    double Longitude
);