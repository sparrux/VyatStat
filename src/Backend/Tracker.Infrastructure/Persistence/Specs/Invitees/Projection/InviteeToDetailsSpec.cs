using Ardalis.Specification;
using Tracker.Application.Contracts.Invitees.Responses;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Application.Contracts.Users.Responses;
using Tracker.Domain.Events.Invitees;

namespace Tracker.Infrastructure.Persistence.Specs.Invitees.Projection;

sealed class InviteeToDetailsSpec : Specification<EventInvitee, EventInviteeDetailsResponse>
{
    public InviteeToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventInviteeDetailsResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id, 
                    x.User.Nickname, 
                    x.User.CreatedAt),
                x.RequirementCompletions.Select(c => 
                    new RequirementCompletionResponse(
                        c.Id,
                        new EventRequirementResponse(
                            c.Requirement.Id,
                            c.Requirement.Title,
                            c.Requirement.Description,
                            c.Requirement.IsMandatory,
                            c.Requirement.ConfirmationMode),
                        c.CompletionStatus)
                ).ToList(),
                x.RsvpStatus,
                x.AdmissionStatus));
    }
}