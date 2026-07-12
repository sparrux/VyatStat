using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Features.Users.Specifications.Projection;
using Hub.Application.Pipelines;
using Hub.Domain;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Users.Queries.GetById;

sealed class GetUserByIdQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetUserByIdQuery, UserDetailsResponse>
{
    public async Task<Result<UserDetailsResponse>> Handle(
        GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .WithSpecification(new GetByIdSpec<User>(query.UserId))
            .WithSpecification(new UserToDetailsSpec())
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return Result.NotFound("User not found by id");
        
        return Result.Success(user);
    }
}