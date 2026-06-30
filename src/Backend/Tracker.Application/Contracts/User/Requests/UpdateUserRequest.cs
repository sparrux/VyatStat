namespace Tracker.Application.Contracts.User.Requests;

public sealed record UpdateUserRequest(
    string NewNickname
);