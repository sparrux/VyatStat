namespace Tracker.Application.Contracts.Events.Requests;

public sealed record EventLocationRequest(
    string? Name,
    double Latitude,
    double Longitude
);