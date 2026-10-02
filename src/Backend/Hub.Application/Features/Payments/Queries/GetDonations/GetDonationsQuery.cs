using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Payments.Queries.GetDonations;

public sealed record GetDonationsQuery(
    Guid UserId,
    int Take = 0,
    int Skip = 0
) : GetListQuery(Take, Skip);