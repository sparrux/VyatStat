using Ardalis.Result;
using Hub.Domain.Events;
using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier.Appliers;

sealed record ApplyContext(
    Event Event,
    EventRequirement Requirement
);

interface IRequirementVerifierApplier
{
    bool CanApply(CreateRequirementVerifierRequest request);
    
    Task<Result<EventRequirementVerifier>> ApplyAsync(
        ApplyContext context,
        CreateRequirementVerifierRequest request,
        CancellationToken cancellationToken);
}

abstract class RequirementVerifierApplierBase<TRequest> : IRequirementVerifierApplier
where TRequest : CreateRequirementVerifierRequest
{
    public bool CanApply(CreateRequirementVerifierRequest request) => request is TRequest;

    public Task<Result<EventRequirementVerifier>> ApplyAsync(
        ApplyContext context,
        CreateRequirementVerifierRequest request,
        CancellationToken cancellationToken
    ) => OnApplyAsync(context, (TRequest)request, cancellationToken);

    protected abstract Task<Result<EventRequirementVerifier>> OnApplyAsync(
        ApplyContext context,
        TRequest request,
        CancellationToken cancellationToken);
}