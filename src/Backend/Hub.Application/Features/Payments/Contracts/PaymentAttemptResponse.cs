using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record PaymentAttemptResponse(
    Guid Id,
    string ProviderName,
    int AttemptNumber,
    PaymentAttemptStatus Status
)
{
    public static PaymentAttemptResponse From(PaymentAttempt attempt) =>
        new(
            attempt.Id,
            attempt.Provider.Value,
            attempt.AttemptNumber,
            attempt.Status
        );
};