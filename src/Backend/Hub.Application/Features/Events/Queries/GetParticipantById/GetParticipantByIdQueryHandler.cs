using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Projection;
using Hub.Application.Features.Events.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Queries.GetParticipantById;

sealed class GetParticipantByIdQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetParticipantByIdQuery, EventParticipantDetailsResponse>
{
    public async Task<Result<EventParticipantDetailsResponse>> Handle(
        GetParticipantByIdQuery request, CancellationToken cancellationToken)
    {
        var participant = await dbContext.EventParticipants
            .WithSpecification(new GetParticipantByEventIdSpec(request.EventId))
            .WithSpecification(new GetParticipantByUserIdSpec(request.ParticipantUserId))
            .WithSpecification(new ParticipantToDetailsSpec())
            .FirstOrDefaultAsync(cancellationToken);

        if (participant is null) return Result.NotFound("Participant not found by user id or event id");
        
        return Result.Success(participant);
    }
}
