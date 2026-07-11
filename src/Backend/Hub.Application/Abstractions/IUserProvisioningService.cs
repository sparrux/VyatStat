using Ardalis.Result;
using Hub.Domain;

namespace Hub.Application.Abstractions;

public sealed record UserProvisioningParameters(string Nickname);

public interface IUserProvisioningService
{
    Task<Result<User>> EnsureCreatedAsync(
        Guid userId,
        UserProvisioningParameters parameters,
        CancellationToken cancellationToken = default);
}
