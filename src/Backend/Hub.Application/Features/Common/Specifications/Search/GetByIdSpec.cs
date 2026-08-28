using Ardalis.Specification;
using Hub.Domain.Common;

namespace Hub.Application.Features.Common.Specifications.Search;

public sealed class GetByIdSpec<T> : Specification<T> where T : Entity
{
    public GetByIdSpec(Guid id)
    {
        Query.Where(e => e.Id == id);
    }
}