using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain;
using Hub.Domain.Events;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateOrganizer;

sealed class CreateOrganizerCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<CreateOrganizerCommand, EventOrganizerSummaryResponse>
{
    public async Task<Result<EventOrganizerSummaryResponse>> Handle(
        CreateOrganizerCommand command, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new GetByIdSpec<Event>(command.EventId))
            .WithSpecification(new EventWithOrganizersSpec())
            .FirstOrDefaultAsync(cancellationToken);
        
        if (ev is null) return Result.NotFound("Event not found by id");
        
        var user = await dbContext.Users
            .WithSpecification(new GetByIdSpec<User>(command.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return Result.NotFound("User not found by id");

        var organizerResult = ev.AddOrganizer(user);

        if (!organizerResult.IsSuccess) return organizerResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new EventOrganizerSummaryResponse(
            organizerResult.Value.Id,
            new UserSummaryResponse(
                user.Id,
                user.Nickname)
        ));
    }
}