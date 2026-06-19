using FluentResults;
using Identity.WebAPI.Contracts;

namespace Identity.WebAPI.Services.Users;

public interface IUsersService
{
    Task<Result<UserResponse>> GetUserAsync(Guid userId);
    Task<Result<UsersResponse>> GetUsersAsync(int take, int skip);
    Task<Result<UserResponse>> CreateAsync(RegistrationRequest request);
    Task<Result<UserClaimsResponse>> GetUserClaimsAsync(Guid userId);
    Task<Result<UserClaimsResponse>> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request);
    Task<Result> SetLockOutAsync(Guid userId, bool isLocked);
}