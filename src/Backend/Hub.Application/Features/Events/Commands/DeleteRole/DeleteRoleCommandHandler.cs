using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteRole;

sealed class DeleteRoleCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<DeleteRoleCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithRolesSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var role = ev.Roles.FirstOrDefault(x => x.Id == command.RoleId);

        if (role is null) return Result.NotFound("Event Role not found by id");

        var removeResult = ev.RemoveRole(role);
        if (!removeResult.IsSuccess) return removeResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}
