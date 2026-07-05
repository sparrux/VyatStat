using Ardalis.Specification;
using Tracker.Domain.Common;

namespace Tracker.Infrastructure.Persistence.Specs.Common.Search;

sealed class ByIdSpec<T> : Specification<T>
    where T : Entity
{
    public ByIdSpec(Guid id)
    {
        Query.Where(e => e.Id == id);
    }
}