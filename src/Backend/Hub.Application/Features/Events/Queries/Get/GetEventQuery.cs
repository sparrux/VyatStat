using Hub.Application.Features.Common.Contracts;
using Hub.Domain.Events;

namespace Hub.Application.Features.Events.Queries.Get;

public sealed record GetEventQuery(
    Guid? OrganizerUserId,
    Guid? GroupId,
    Guid? ParticipantUserId,
    EventState? State,
    int Take = 0,
    int Skip = 0
) : GetListQuery(Take, Skip);