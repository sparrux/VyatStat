using Ardalis.Specification;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain.Events.Invitees;

namespace Hub.Application.Features.Events.Specifications.Projection;

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
                    x.User.Nickname),
                x.RequirementCompletions.Select(
                    c => new RequirementCompletionResponse(
                        c.Id,
                        new EventRequirementSummaryResponse(
                            c.Requirement.Id,
                            c.Requirement.Title,
                            c.Requirement.Description,
                            c.Requirement.IsMandatory,
                            c.Requirement.VerificationMode),
                        c.VerificationStatus)).ToList(),
                x.RsvpStatus,
                x.AdmissionStatus));
    }
}