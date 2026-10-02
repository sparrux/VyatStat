using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Payments.Queries.GetInvoices;

public sealed record GetInvoicesQuery(
    Guid UserId,
    int Take = 0,
    int Skip = 0
) : GetListQuery(Take, Skip);