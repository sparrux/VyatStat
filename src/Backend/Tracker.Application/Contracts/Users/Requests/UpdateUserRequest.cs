namespace Tracker.Application.Contracts.Users.Requests;

public sealed record UpdateUserRequest(
    string NewNickname
);