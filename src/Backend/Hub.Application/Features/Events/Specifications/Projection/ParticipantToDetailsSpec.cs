using Ardalis.Specification;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Users.Contracts;
using Hub.Domain.Events.Participants;

namespace Hub.Application.Features.Events.Specifications.Projection;

sealed class ParticipantToDetailsSpec : Specification<EventParticipant, EventParticipantDetailsResponse>
{
    public ParticipantToDetailsSpec()
    {
        Query
            .AsNoTracking()
            .Select(x => new EventParticipantDetailsResponse(
                x.Id,
                new UserSummaryResponse(
                    x.User.Id,
                    x.User.Nickname),
                x.Requirements.Select(
                    c => new RequirementCompletionResponse(
                        c.Id,
                        new EventRequirementSummaryResponse(
                            c.Requirement.Id,
                            c.Requirement.Title,
                            c.Requirement.Description))).ToList()));
    }
}
