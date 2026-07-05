using FluentResults;
using Tracker.Application.Contracts.Events.Requests;
using Tracker.Application.Contracts.Events.Responses;

namespace Tracker.Application.Interfaces.Events;

public interface IEventsService
{
    Task<Result<EventSummaryResponse>> CreateAsync(Guid userId, CreateEventRequest request, CancellationToken ctk = default);
    Task<Result<EventsListResponse>> GetListAsync(Guid organizerId, int offset, int take, CancellationToken ctk = default);
    Task<Result<EventDetailsResponse>> GetAsync(Guid organizerId, CancellationToken ctk = default);
    Task<Result> UpdateTitleAsync(Guid eventId, UpdateEventTitleRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDescriptionAsync(Guid eventId, UpdateEventDescriptionRequest request, CancellationToken ctk = default);
    Task<Result> UpdateDatesAsync(Guid eventId, UpdateEventDatesRequest request, CancellationToken ctk = default);
    Task<Result> UpdateLocationAsync(Guid eventId, UpdateEventLocationRequest request, CancellationToken ctk = default);
    Task<Result> DeleteAsync(Guid eventId, CancellationToken ctk = default);

    Task<Result> AttachToGroupAsync(Guid eventId, Guid groupId, CancellationToken ctk = default);
    Task<Result> DetachFromGroupAsync(Guid eventId, Guid groupId, CancellationToken ctk = default);
}