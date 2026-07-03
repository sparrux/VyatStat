using FluentResults;
using Tracker.Application.Contracts.Event.Responses;

namespace Tracker.Application.Services.Invitees;

public interface IInviteesService
{
    Task<Result<GroupEventInviteeSummaryResponse>> CreateAsync(Guid eventId, Guid userId, CancellationToken ctk = default);
    Task<Result<GroupEventInviteesListResponse>> GetListAsync(Guid eventId, int offset, int take, CancellationToken ctk = default);
    Task<Result<GroupEventInviteeDetailsResponse>> GetAsync(Guid eventId, Guid userId, CancellationToken ctk = default);
}