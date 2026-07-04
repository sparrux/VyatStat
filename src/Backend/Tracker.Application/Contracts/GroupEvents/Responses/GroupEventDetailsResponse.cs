using Tracker.Application.Contracts.Common.Responses;
using Tracker.Application.Contracts.Organizers.Responses;
using Tracker.Application.Contracts.Requirements.Responses;
using Tracker.Domain.GroupEvents;

namespace Tracker.Application.Contracts.GroupEvents.Responses;

public sealed record GroupEventDetailsResponse(
    Guid Id,
    Guid GroupId,
    string Title,
    FormatTextResponse Description,
    DateTimeOffset EndDate,
    DateTimeOffset StartDate,
    GroupEventState State,
    LocationResponse? Location,
    IReadOnlyCollection<GroupEventOrganizerResponse> Organizers,
    IReadOnlyCollection<GroupEventRequirementResponse> Requirements
);