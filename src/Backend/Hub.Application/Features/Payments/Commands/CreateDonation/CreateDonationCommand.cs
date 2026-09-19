namespace Hub.Application.Features.Payments.Commands.CreateDonation;

public sealed record CreateDonationCommand(
    Guid UserId,
    CreateDonationRequest Request
);

public sealed record CreateDonationRequest(
    decimal Amount,
    string Currency,
    string IdempotencyKey,
    bool IsAnonymous = false,
    string? Provider = null,
    string? Description = null,
    Uri? ReturnUrl = null,
    Uri? CancelUrl = null
);
