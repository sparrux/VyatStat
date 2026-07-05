using Ardalis.Specification;
using Tracker.Domain.Common;

namespace Tracker.Infrastructure.Persistence.Specs.Common.Ordering;

sealed class UpdatedAtOrderingSpec<T> : Specification<T>
    where T : Auditable
{
    public UpdatedAtOrderingSpec(bool descending = true)
    {
        if (descending)
        {
            Query.OrderByDescending(e => e.UpdatedAt);
        }
        else
        {
            Query.OrderBy(e => e.UpdatedAt);
        }
    }
}