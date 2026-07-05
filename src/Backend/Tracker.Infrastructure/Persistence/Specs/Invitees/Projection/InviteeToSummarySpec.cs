using Ardalis.Specification;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees.Projection;

sealed class InviteeToSummarySpec : Specification<EventInvitee, EventInviteeSummaryResponse>
{
    public InviteeToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventInviteeSummaryResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id, 
                    x.User.Nickname, 
                    x.User.CreatedAt)));
    }
}