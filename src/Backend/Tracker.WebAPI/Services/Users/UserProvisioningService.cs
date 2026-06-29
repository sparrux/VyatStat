using FluentResults;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tracker.Application.Services.Users;
using Tracker.Domain;
using Tracker.Infrastructure.Persistence;

namespace Tracker.WebAPI.Services.Users;

public sealed class UserProvisioningService(AppDbContext dbContext) : IUserProvisioningService
{
    public async Task<Result> EnsureCreatedAsync(
        Guid userId, 
        UserCreationParameters creationParameters, 
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Users
            .AnyAsync(u => u.Id == userId, cancellationToken);

        if (existing)
            return Result.Ok();

        var nickname = creationParameters.Nickname;
        
        if (string.IsNullOrWhiteSpace(nickname))
            return Result.Fail("Invalid nickname");
        
        var createResult = User.Create(userId, nickname);

        if (createResult.IsFailed)
            return createResult.ToResult();

        var user = createResult.Value;

        try
        {
            await dbContext.Users.AddAsync(user, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Fail(new ExceptionalError(ex));
        }
        
        return Result.Ok();
    }
}
