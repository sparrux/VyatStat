using Ardalis.Specification;
using Hub.Domain.Events.Requirements;

namespace Hub.Application.Features.Events.Specifications.Include;

sealed class RequirementWithVerifiersSpec : Specification<EventRequirement>
{
    public RequirementWithVerifiersSpec()
    {
        Query
            .Include(x => x.Verifiers)
                .ThenInclude(v => (v as EventRequirementRoleVerifier)!.Verifier)
            .Include(x => x.Verifiers)
                .ThenInclude(v => (v as EventRequirementParticipantVerifier)!.Verifier)
                    .ThenInclude(p => p.User);
    }
}