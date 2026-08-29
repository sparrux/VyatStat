using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteParticipantRole;

sealed class DeleteParticipantRoleCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<DeleteParticipantRoleCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteParticipantRoleCommand command, CancellationToken cancellationToken)
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

        var participantRole = participant.Roles.FirstOrDefault(x => x.RoleId == command.RoleId);
        if (participantRole is null) return Result.NotFound("Participant Role not found");

        var removeResult = ev.RemoveParticipantRole(role, participantRole);
        if (!removeResult.IsSuccess) return removeResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}
