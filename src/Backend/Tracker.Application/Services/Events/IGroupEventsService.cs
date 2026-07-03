using FluentResults;
using Tracker.Application.Contracts.Event.Requests;
using Tracker.Application.Contracts.Event.Responses;

namespace Tracker.Application.Services.Events;

public interface IGroupEventsService
{
    Task<Result<GroupEventSummaryResponse>> CreateAsync(Guid groupId, Guid orgId, CreateGroupEventRequest request, CancellationToken ctk = default);
    Task<Result<GroupEventsListResponse>> GetListAsync(Guid groupId, int offset, int take, CancellationToken ctk = default);
    Task<Result<GroupEventDetailsResponse>> GetAsync(Guid eventId, CancellationToken ctk = default);
    Task<Result> UpdateTitleAsync(Guid eventId, UpdateGroupEventTitleRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDescriptionAsync(Guid eventId, UpdateGroupEventDescriptionRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDatesAsync(Guid eventId, UpdateGroupEventDatesRequest request, CancellationToken ctk = default);
    Task<Result> UpdateLocationAsync(Guid eventId, UpdateGroupEventLocationRequest request, CancellationToken ctk = default);
    Task<Result<GroupEventRequirementResponse>> CreateRequirementAsync(Guid eventId, CreateGroupEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result<GroupEventRequirementResponse>> UpdateRequirementAsync(Guid eventId, Guid reqId, UpdateGroupEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result> DeleteRequirementAsync(Guid eventId, Guid reqId, CancellationToken ctk = default);
    Task<Result> DeleteAsync(Guid eventId, CancellationToken ctk = default);
}