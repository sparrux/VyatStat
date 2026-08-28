using Ardalis.Result;
using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier.Appliers;

sealed class RequirementRuleVerifierApplier
    : RequirementVerifierApplierBase<CreateRequirementRuleVerifierRequest>
{
    protected override Task<Result<EventRequirementVerifier>> OnApplyAsync(
        ApplyContext context, 
        CreateRequirementRuleVerifierRequest request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}