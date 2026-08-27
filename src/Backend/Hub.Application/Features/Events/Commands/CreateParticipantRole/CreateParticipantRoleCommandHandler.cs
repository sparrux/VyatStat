using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateParticipantRole;

sealed class CreateParticipantRoleCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<CreateParticipantRoleCommand, EventParticipantRoleResponse>
{
    public async Task<Result<EventParticipantRoleResponse>> Handle(
        CreateParticipantRoleCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithParticipantRolesSpec())
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");

        var role = ev.Roles.FirstOrDefault(x => x.Id == command.RoleId);
        if (role is null) return Result.NotFound("Event Role not found by id");

        var participant = ev.Participants.FirstOrDefault(x => x.UserId == command.UserId);
        if (participant is null) return Result.NotFound("Participant not found by user id");

        var addResult = ev.AddParticipantRole(role, participant);
        if (!addResult.IsSuccess) return addResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        var participantRole = addResult.Value;
        return Result.Created(new EventParticipantRoleResponse(
            participantRole.Id,
            new EventRoleSummaryResponse(
                role.Id,
                role.Name,
                role.IsSealed)));
    }
}
