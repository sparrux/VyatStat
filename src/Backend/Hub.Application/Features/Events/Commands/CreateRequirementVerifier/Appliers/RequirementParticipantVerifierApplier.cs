using Ardalis.Result;
using Hub.Domain.Events.Requirements;
using Hub.Infrastructure.Persistence;

namespace Hub.Application.Features.Events.Commands.CreateRequirementVerifier.Appliers;

sealed class RequirementParticipantVerifierApplier(
    HubDbContext dbContext
) : RequirementVerifierApplierBase<CreateRequirementParticipantVerifierRequest>
{
    protected override async Task<Result<EventRequirementVerifier>> OnApplyAsync(
        ApplyContext context, 
        CreateRequirementParticipantVerifierRequest request,
        CancellationToken cancellationToken)
    {
        await dbContext
            .Entry(context.Event)
            .Collection(x => x.Participants)
            .LoadAsync(cancellationToken);
            
        var participant = context.Event.Participants.FirstOrDefault(x => x.UserId == request.ParticipantUserId);
        if (participant is null) return Result.NotFound("Event Participant not found");
            
        return context.Event
            .AddRequirementParticipantVerifier(context.Requirement, participant, request.IsRequired)
            .Map(EventRequirementVerifier (x) => x);

    }
}