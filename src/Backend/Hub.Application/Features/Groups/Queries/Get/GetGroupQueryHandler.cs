using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Groups.Contracts;
using Hub.Application.Features.Groups.Specifications.Ordering;
using Hub.Application.Features.Groups.Specifications.Projection;
using Hub.Application.Features.Groups.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain.Groups;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Groups.Queries.Get;

sealed class GetGroupQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetGroupQuery, ListResponse<GroupSummaryResponse>>
{
    public async Task<Result<ListResponse<GroupSummaryResponse>>> Handle(GetGroupQuery query, CancellationToken cancellationToken)
    {
        var spec = new GetGroupByQuerySpec(query);
        
        var groups = await dbContext.Groups
            .WithSpecification(spec)
            .WithSpecification(new ListSelectionSpec<Group>(query.Take, query.Skip))
            .WithSpecification(new GroupOrderingSpec())
            .WithSpecification(new GroupToSummarySpec())
            .ToListAsync(cancellationToken);
        
        return Result.Success(
            new ListResponse<GroupSummaryResponse>(
                groups,
                await dbContext.Groups
                    .WithSpecification(spec)
                    .CountAsync(cancellationToken)));
    }
}