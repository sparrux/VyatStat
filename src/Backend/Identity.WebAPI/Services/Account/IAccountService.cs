using FluentResults;
using Identity.WebAPI.Contracts;

namespace Identity.WebAPI.Services.Account;

public interface IAccountService
{
    Task<Result<ProfileResponse>> CreateAsync(RegistrationRequest request);
    Task<Result<ProfileResponse>> GetProfileAsync(Guid userId);
    Task<Result<UserClaimsResponse>> GetUserClaimsAsync(Guid userId);
    Task<Result<UserClaimsResponse>> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request);
}