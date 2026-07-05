using FluentResults;
using Tracker.Application.Contracts.Users.Requests;
using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Interfaces.Users;

public interface IUsersService
{
    Task<Result<UsersListResponse>> GetListAsync(int offset, int take, CancellationToken ctk = default);
    Task<Result<UserDetailsResponse>> GetAsync(Guid userId, CancellationToken ctk = default);
    Task<Result> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken ctk = default);
}