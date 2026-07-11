using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Events.Contracts;

public sealed record EventOrganizerResponse(
    Guid Id,
    UserSummaryResponse User
);