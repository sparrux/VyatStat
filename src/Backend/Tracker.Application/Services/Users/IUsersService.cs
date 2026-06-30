using FluentResults;
using Tracker.Application.Contracts.User.Requests;
using Tracker.Application.Contracts.User.Responses;

namespace Tracker.Application.Services.Users;

public interface IUsersService
{
    Task<Result<UsersListResponse>> GetListAsync(int offset, int take, CancellationToken ctk = default);
    Task<Result<UserSummaryResponse>> GetSummaryAsync(Guid userId, CancellationToken ctk = default);
    Task<Result<UserDetailsResponse>> GetDetailsAsync(Guid userId, CancellationToken ctk = default);
    Task<Result> UpdateAsync(Guid userId, UpdateUserRequest request, CancellationToken ctk = default);
}