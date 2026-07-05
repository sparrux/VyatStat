using Ardalis.Specification;
using Tracker.Domain.Common;

namespace Tracker.Infrastructure.Persistence.Specs.Common.Ordering;

sealed class CreatedAtOrderingSpec<T> : Specification<T>
    where T : Auditable
{
    public CreatedAtOrderingSpec(bool descending = true)
    {
        if (descending)
        {
            Query.OrderByDescending(e => e.CreatedAt);
        }
        else
        {
            Query.OrderBy(e => e.CreatedAt);
        }
    }
}