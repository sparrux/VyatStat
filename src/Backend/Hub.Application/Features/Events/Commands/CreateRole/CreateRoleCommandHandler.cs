using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateRole;

sealed class CreateRoleCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<CreateRoleCommand, EventRoleSummaryResponse>
{
    public async Task<Result<EventRoleSummaryResponse>> Handle(
        CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRolesSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var addResult = ev.AddRole(command.Request.Name, command.Request.IsSealed);
        if (!addResult.IsSuccess) return addResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        var role = addResult.Value;
        return Result.Created(new EventRoleSummaryResponse(
            role.Id,
            role.Name,
            role.IsSealed));
    }
}
