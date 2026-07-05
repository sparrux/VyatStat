using FluentResults;

namespace Tracker.Application.Interfaces.Users;

public sealed record UserCreationParameters(string Nickname);

public interface IUserProvisioningService
{
    Task<Result> EnsureCreatedAsync(
        Guid userId, 
        UserCreationParameters creationParameters, 
        CancellationToken cancellationToken = default);
}
