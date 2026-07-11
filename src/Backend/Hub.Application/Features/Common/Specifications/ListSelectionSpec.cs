using Ardalis.Specification;
using Hub.Domain.Common;

namespace Hub.Application.Features.Common.Specifications;

sealed class ListSelectionSpec<T> : Specification<T> where T : Entity
{
    public ListSelectionSpec(int take, int skip)
    {
        Query.Skip(skip).Take(take);
    }
}