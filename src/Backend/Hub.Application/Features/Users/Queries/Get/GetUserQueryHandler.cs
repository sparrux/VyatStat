using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Features.Users.Specifications.Projection;
using Hub.Application.Features.Users.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Domain;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Users.Queries.Get;

sealed class GetUserQueryHandler(
    IHubDbContext dbContext
) : IRequestHandler<GetUserQuery, ListResponse<UserSummaryResponse>>
{
    public async Task<Result<ListResponse<UserSummaryResponse>>> Handle(
        GetUserQuery query, CancellationToken cancellationToken)
    {
        var search = new GetUserByQuerySpec(query);
        
        var users = await dbContext.Users
            .WithSpecification(search)
            .WithSpecification(new ListSelectionSpec<User>(query.Take, query.Skip))
            .WithSpecification(new UserToSummarySpec())
            .ToListAsync(cancellationToken);
        
        return Result.Success(new ListResponse<UserSummaryResponse>(
            users,
            await dbContext.Users.WithSpecification(search).CountAsync(cancellationToken)));
    }
}