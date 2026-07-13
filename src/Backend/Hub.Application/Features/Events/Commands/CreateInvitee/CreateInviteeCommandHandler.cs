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

namespace Hub.Application.Features.Events.Commands.CreateInvitee;

sealed class CreateInviteeCommandHandler(
    HubDbContext dbContext
) : IRequestHandler<CreateInviteeCommand, EventInviteeSummaryResponse>
{
    public async Task<Result<EventInviteeSummaryResponse>> Handle(
        CreateInviteeCommand request, CancellationToken cancellationToken)
    {
        var ev = await dbContext.Events
            .WithSpecification(new EventWithInviteesSpec())
            .WithSpecification(new EventWithRequirementsSpec())
            .WithSpecification(new EventWithRequirementCompletionsSpec())
            .WithSpecification(new GetByIdSpec<Event>(request.EventId))
            .FirstOrDefaultAsync(cancellationToken);

        if (ev is null) return Result.NotFound("Event not found by id");
        
        var user = await dbContext.Users
            .WithSpecification(new GetByIdSpec<User>(request.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null) return Result.NotFound("User not found by id");

        var inviteeResult = ev.AddInvitee(user);

        if (!inviteeResult.IsSuccess) return inviteeResult.Map();

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new EventInviteeSummaryResponse(
            inviteeResult.Value.Id,
            new UserSummaryResponse(
                user.Id,
                user.Nickname)));
    }
}