using FluentResults;
using Tracker.Application.Contracts.Common.Requests;
using Tracker.Application.Contracts.Organizers.Requests;
using Tracker.Application.Contracts.Organizers.Responses;

namespace Tracker.Application.Interfaces.Organizers;

public interface IEventOrganizersService
{
    Task<Result<EventOrganizerResponse>> CreateAsync(
        Guid eventId, 
        CreateEventOrganizerRequest request, 
        CancellationToken ctk = default);
    
    Task<Result<EventOrganizerResponse>> GetListAsync(
        EventOrganizerFilterRequest request, 
        ListSelectionRequest selection,
        CancellationToken ctk = default);
    
    Task<Result<EventOrganizerResponse>> GetAsync(Guid organizerId, CancellationToken ctk = default);
    
    Task<Result> DeleteAsync(Guid organizerId, CancellationToken ctk = default);
}