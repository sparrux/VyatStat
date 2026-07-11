namespace Hub.Application.Features.Events.Contracts;

public sealed record EventLocationResponse(
    Guid Id,
    string? Name,
    double Latitude,
    double Longitude
);