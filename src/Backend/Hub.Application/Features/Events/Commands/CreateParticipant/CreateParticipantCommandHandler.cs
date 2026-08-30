using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications.Search;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain;
using Hub.Domain.Events;
using Hub.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Commands.CreateParticipant;

sealed class CreateParticipantCommandHandler(
    IHubDbContext dbContext
) : IRequestHandler<CreateParticipantCommand, EventParticipantSummaryResponse>
{
    public async Task<Result<EventParticipantSummaryResponse>> Handle(
        CreateParticipantCommand request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithParticipantsSpec())
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementAssignmentsSpec())
            .WithSpecification(new GetByIdSpec<Event>(request.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");
        
        var user = await dbContext.Users
            .WithSpecification(new GetByIdSpec<User>(request.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return Result.NotFound("User not found by id");

        var participantResult = ev.AddParticipant(user);

        if (!participantResult.IsSuccess) return participantResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new EventParticipantSummaryResponse(
            new UserSummaryResponse(
                user.Id,
                user.Nickname)));
    }
}
