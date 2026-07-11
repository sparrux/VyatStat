namespace Hub.Application.Features.Common.Contracts;

public sealed record UserSummaryResponse(
    Guid Id,
    string Nickname
);