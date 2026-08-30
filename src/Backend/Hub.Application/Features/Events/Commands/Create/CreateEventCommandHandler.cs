using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain;
using Hub.Domain.Events;
using Hub.Domain.ValueObjects;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.Create;

sealed class CreateEventCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<CreateEventCommand, EventSummaryResponse>
{
    public async Task<Result<EventSummaryResponse>> Handle(
        CreateEventCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .WithSpecification(new GetByIdSpec<User>(command.OrganizerUserId))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (user is null) return Result.NotFound("User not found");

        var request = command.Request;
        
        var createEvent = Event.CreateDraft(
            user,
            request.Title, 
            new DatesRange(request.Dates.StartDate, request.Dates.EndDate));

        if (!createEvent.IsSuccess) return createEvent.Map();
        
        var ev = createEvent.Value;

        await dbContext.Events.AddAsync(ev, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Created(new EventSummaryResponse(
            ev.Id,
            ev.Title,
            ev.State,
            ev.DatesRange.EndDate,
            ev.DatesRange.StartDate,
            ev.Location is not null,
            ev.Participants.Count,
            ev.Requirements.Count,
            ev.Goals.Count,
            ev.Roles.Count
        ));
    }
}