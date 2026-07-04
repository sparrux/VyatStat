using Tracker.Application.Contracts.Users.Responses;

namespace Tracker.Application.Contracts.GroupMembers.Responses;

public sealed record GroupMemberSummaryResponse(
    UserSummaryResponse User,
    Guid GroupId
);