using Ardalis.Specification;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.GroupEvents.Invitees;

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
                            c.Requirement.IsMandatory),
                        c.CompletionStatus)
                ).ToList(),
                x.RsvpStatus,
                x.AdmissionStatus));
    }
}