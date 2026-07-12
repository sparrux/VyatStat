using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Specifications;
using Hub.Domain;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Web.Services.Users;

sealed class UserProvisioningService(HubDbContext dbContext) : IUserProvisioningService
{
    public async Task<Result> EnsureCreatedAsync(
        Guid userId,
        UserProvisioningParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Users
            .AsNoTracking()
            .WithSpecification(new GetByIdSpec<User>(userId))
            .AnyAsync(cancellationToken);
        
        if (existing)
            return Result.Success();

        var nickname = parameters.Nickname;
        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Error("Nickname is required to create a user.");

        var createResult = User.Create(userId, nickname);
        if (!createResult.IsSuccess)
            return createResult.Map();

        await dbContext.AddAsync(createResult.Value, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return createResult.Map();
    }
}
