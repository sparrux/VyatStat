using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Groups.Queries.Get;

public sealed record GetGroupQuery(
    Guid? MemberUserId,
    int Take,
    int Skip
) : GetListQuery(Take, Skip);