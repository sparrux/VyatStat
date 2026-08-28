namespace Hub.Application.Features.Events.Contracts;

public sealed record EventRoleSummaryResponse(
    Guid Id,
    string Name,
    bool IsSealed
);
