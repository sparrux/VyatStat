using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Commands.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid EventId,
    UpdateLocationRequest Request
);

public sealed record UpdateLocationRequest(
    string? Name,
    double Latitude,
    double Longitude
) : LocationModel(Latitude, Longitude);