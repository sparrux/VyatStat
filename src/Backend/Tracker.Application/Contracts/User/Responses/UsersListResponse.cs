namespace Tracker.Application.Contracts.User.Responses;

public sealed record UsersListResponse(
    IReadOnlyCollection<UserSummaryResponse> Users,
    int TotalCount
);