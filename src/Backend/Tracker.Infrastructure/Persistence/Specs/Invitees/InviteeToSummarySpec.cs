using Ardalis.Specification;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Application.Contracts.User.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees;

sealed class InviteeToSummarySpec : Specification<GroupEventInvitee, GroupEventInviteeSummaryResponse>
{
    public InviteeToSummarySpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new GroupEventInviteeSummaryResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id, 
                    x.User.Nickname, 
                    x.User.CreatedAt)));
    }
}