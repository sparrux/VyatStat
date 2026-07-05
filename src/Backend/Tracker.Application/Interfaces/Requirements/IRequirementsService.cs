using FluentResults;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Contracts.Requirements.Responses;

namespace Tracker.Application.Interfaces.Requirements;

public interface IRequirementsService
{
    Task<Result<EventRequirementResponse>> CreateAsync(Guid eventId, CreateEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result<EventRequirementResponse>> UpdateAsync(Guid eventId, Guid reqId, UpdateEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result> DeleteAsync(Guid eventId, Guid reqId, CancellationToken ctk = default);
}