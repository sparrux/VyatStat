namespace Tracker.Application.Contracts.Common.Requests;

public sealed record LocationRequest(
    string? Name,
    double Latitude,
    double Longitude
);