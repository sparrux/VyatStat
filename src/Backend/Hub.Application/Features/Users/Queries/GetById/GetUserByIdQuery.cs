namespace Hub.Application.Features.Users.Queries.GetById;

public sealed record GetUserByIdQuery(
    Guid UserId
);