using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Tracker.Application.Contracts.User.Requests;
using Tracker.Application.Contracts.User.Responses;
using Tracker.Application.Services.Users;
using Tracker.Domain;
using Tracker.Infrastructure.Persistence;
using Tracker.Infrastructure.Persistence.Specs.Common;
using Tracker.Infrastructure.Persistence.Specs.Users;

namespace Tracker.Infrastructure.Services.Users;

public sealed class UsersService(AppDbContext context) : IUsersService
{
    public async Task<Result<UsersListResponse>> GetListAsync(int offset, int take, CancellationToken ctk = default)
    {
        var ordering = new CreatedAtOrderingSpec<User>();
        
        var projection = 
            new SelectionSpec<User>(offset, take)
                .WithProjectionOf(new UserToSummarySpec());
        
        var summaryList = await context.Users
            .WithSpecification(ordering)
            .WithSpecification(projection)
            .ToListAsync(ctk);
        
        return Result.Ok(new UsersListResponse(summaryList, await context.Users.CountAsync(ctk)));
    }

    public async Task<Result<UserSummaryResponse>> GetSummaryAsync(Guid userId, CancellationToken ctk = default)
    {
        var projection = 
            new ByIdSpec<User>(userId)
                .WithProjectionOf(new UserToSummarySpec());
        
        var summary = await context.Users
            .WithSpecification(projection)
            .FirstOrDefaultAsync(ctk);
        
        if (summary is null)
            return Result.Fail("User not found");
        
        return Result.Ok(summary);
    }

    public async Task<Result<UserDetailsResponse>> GetDetailsAsync(Guid userId, CancellationToken ctk = default)
    {
        var projection = 
            new ByIdSpec<User>(userId)
                .WithProjectionOf(new UserToDetailsSpec());
        
        var details = await context.Users
            .WithSpecification(projection)
            .FirstOrDefaultAsync(ctk);
        
        if (details is null)
            return Result.Fail("User not found");
        
        return Result.Ok(details);
    }

    public async Task<Result> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken ctk = default)
    {
        var spec = new ByIdSpec<User>(userId);

        var rows = await context.Users
            .WithSpecification(spec)
            .ExecuteUpdateAsync(x =>
                x.SetProperty(e => e.Nickname, request.NewNickname), ctk);
        
        return Result.FailIf(rows <= 0, "Failed to update user nickname");
    }
}