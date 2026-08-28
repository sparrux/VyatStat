using Ardalis.Result;
using Hub.Domain.Events.Requirements;
using Hub.Infrastructure.Persistence;

namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier.Appliers;

sealed class RequirementRoleVerifierApplier(
    HubDbContext dbContext
) : RequirementVerifierApplierBase<CreateRequirementRoleVerifierRequest>
{
    protected override async Task<Result<EventRequirementVerifier>> OnApplyAsync(
        ApplyContext context, 
        CreateRequirementRoleVerifierRequest request,
        CancellationToken cancellationToken)
    {
        await dbContext
            .Entry(context.Event)
            .Collection(x => x.Roles)
            .LoadAsync(cancellationToken);

        var role = context.Event.Roles.FirstOrDefault(x => x.Id == request.RoleId);
        if (role is null) return Result.NotFound("Event Role not found");

        return context.Event
            .AddRequirementRoleVerifier(context.Requirement, role, request.IsRequired)
            .Map(EventRequirementVerifier (x) => x);
    }
}