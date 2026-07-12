using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Users.Queries.Get;

public sealed record GetUserQuery(
    int Take = 0,
    int Skip = 0
) : GetListQuery(Take, Skip);