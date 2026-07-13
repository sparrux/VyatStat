using Ardalis.Result;
using Ardalis.Specification.EntityFrameworkCore;
using Hub.Application.Features.Common.Specifications;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Specifications.Include;
using Hub.Application.Features.Events.Specifications.Projection;
using Hub.Application.Features.Events.Specifications.Search;
using Hub.Application.Pipelines;
using Hub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Events.Queries.GetInviteeById;

sealed class GetInviteeByIdQueryHandler(
    HubDbContext dbContext
) : IRequestHandler<GetInviteeByIdQuery, EventInviteeDetailsResponse>
{
    public async Task<Result<EventInviteeDetailsResponse>> Handle(
        GetInviteeByIdQuery request, CancellationToken cancellationToken)
    {
        var invitee = await dbContext.EventInvitees
            .WithSpecification(new GetInviteeByEventIdSpec(request.EventId))
            .WithSpecification(new GetInviteeByUserIdSpec(request.InviteeUserId))
            .WithSpecification(new InviteeToDetailsSpec())
            .FirstOrDefaultAsync(cancellationToken);

        if (invitee is null) return Result.NotFound("Invitee not found by user id or event id");
        
        return Result.Success(invitee);
    }
}