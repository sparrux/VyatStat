using FluentResults;
using Tracker.Application.Contracts.Requirements.Requests;
using Tracker.Application.Contracts.Requirements.Responses;

namespace Tracker.Application.Services.Requirements;

public interface IRequirementsService
{
    Task<Result<GroupEventRequirementResponse>> CreateAsync(Guid eventId, CreateGroupEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result<GroupEventRequirementResponse>> UpdateAsync(Guid eventId, Guid reqId, UpdateGroupEventRequirementRequest request, CancellationToken ctk = default);
    Task<Result> DeleteAsync(Guid eventId, Guid reqId, CancellationToken ctk = default);

}