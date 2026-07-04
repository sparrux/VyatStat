namespace Tracker.Application.Contracts.Users.Responses;

public sealed record UsersListResponse(
    IReadOnlyCollection<UserSummaryResponse> Users,
    int TotalCount
);