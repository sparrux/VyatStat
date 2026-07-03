using Ardalis.Specification;
using Tracker.Application.Contracts.Event.Responses;
using Tracker.Application.Contracts.User.Responses;
using Tracker.Domain.GroupEvents.Events;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees;

sealed class InviteeToDetailsSpec : Specification<GroupEventInvitee, GroupEventInviteeDetailsResponse>
{
    public InviteeToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new GroupEventInviteeDetailsResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id, 
                    x.User.Nickname, 
                    x.User.CreatedAt),
                x.RequirementCompletions.Select(c => 
                    new InviteeRequirementCompletionResponse(
                        new GroupEventRequirementResponse(
                            c.Requirement.Id,
                            c.Requirement.Title,
                            c.Requirement.Description,
                            c.Requirement.IsMandatory))).ToList(),
                x.RsvpStatus,
                x.AdmissionStatus));
    }
}