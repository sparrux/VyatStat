using FluentResults;
using Tracker.Application.Contracts.Invitees.Responses;

namespace Tracker.Application.Interfaces.Invitees;

public interface IInviteesService
{
    Task<Result<EventInviteeSummaryResponse>> CreateAsync(Guid eventId, Guid userId, CancellationToken ctk = default);
    Task<Result<EventInviteesListResponse>> GetListAsync(Guid eventId, int offset, int take, CancellationToken ctk = default);
    Task<Result<EventInviteeDetailsResponse>> GetAsync(Guid eventId, Guid userId, CancellationToken ctk = default);
}