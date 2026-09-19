using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record CreatePaymentRequest(
    Guid? CustomerId,
    decimal Amount,
    string Currency,
    Guid ReferenceId,
    PaymentPurpose Purpose,
    string IdempotencyKey,
    bool IsAnonymous = false
);

public sealed record PaymentCheckoutRequest(
    Guid ReferenceId,
    string IdempotencyKey,
    string? Provider = null,
    string? Description = null,
    Uri? ReturnUrl = null,
    Uri? CancelUrl = null
);