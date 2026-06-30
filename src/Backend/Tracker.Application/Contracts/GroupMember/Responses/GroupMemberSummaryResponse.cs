using Tracker.Application.Contracts.User.Responses;

namespace Tracker.Application.Contracts.GroupMember.Responses;

public sealed record GroupMemberSummaryResponse(
    Guid Id,
    UserSummaryResponse User,
    Guid GroupId
);