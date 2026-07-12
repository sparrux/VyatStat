using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.DeleteOrganizer;

sealed class DeleteOrganizerCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<DeleteOrganizerCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(
        DeleteOrganizerCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .WithSpecification(new EventWithOrganizersSpec())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ev is null) return Result.NotFound("Event not found by id");
        
        var organizer = ev.Organizers.FirstOrDefault(x => x.UserId == command.UserId);
        if (organizer is null) return Result.NotFound("Organizer not found by user id");

        var removeResult = ev.RemoveOrganizer(organizer);
        if (!removeResult.IsSuccess) return removeResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new IdResponse(ev.Id));
    }
}