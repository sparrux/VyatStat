using Ardalis.Specification;

namespace Tracker.Infrastructure.Persistence.Specs.Common;

sealed class SelectionSpec<T> : Specification<T>
{
    public SelectionSpec(int skip, int take)
    {
        Query
            .Skip(skip)
            .Take(take);
    }
}