namespace Tracker.Application.Contracts.Common.Responses;

public sealed record LocationResponse(
    Guid Id,
    string? Name,
    double Latitude,
    double Longitude
);