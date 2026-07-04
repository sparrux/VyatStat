using FluentResults;
using Tracker.Application.Contracts.GroupEvents.Requests;
using Tracker.Application.Contracts.GroupEvents.Responses;

namespace Tracker.Application.Services.GroupEvents;

public interface IGroupEventsService
{
    Task<Result<GroupEventSummaryResponse>> CreateAsync(Guid groupId, Guid orgId, CreateGroupEventRequest request, CancellationToken ctk = default);
    Task<Result<GroupEventsListResponse>> GetListAsync(Guid groupId, int offset, int take, CancellationToken ctk = default);
    Task<Result<GroupEventDetailsResponse>> GetAsync(Guid eventId, CancellationToken ctk = default);
    Task<Result> UpdateTitleAsync(Guid eventId, UpdateGroupEventTitleRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDescriptionAsync(Guid eventId, UpdateGroupEventDescriptionRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDatesAsync(Guid eventId, UpdateGroupEventDatesRequest request, CancellationToken ctk = default);
    Task<Result> UpdateLocationAsync(Guid eventId, UpdateGroupEventLocationRequest request, CancellationToken ctk = default);
    Task<Result> DeleteAsync(Guid eventId, CancellationToken ctk = default);
}