using Ardalis.Specification;
using Hub.Domain.Common;

namespace Hub.Application.Features.Common.Specifications.Ordering;

sealed class OrderByCreatedAtSpec<T> : Specification<T>
where T : Auditable
{
    public OrderByCreatedAtSpec(bool descending)
    {
        if (descending)
            Query.OrderByDescending(x => x.CreatedAt);
        else
            Query.OrderBy(x => x.CreatedAt);
    }
}