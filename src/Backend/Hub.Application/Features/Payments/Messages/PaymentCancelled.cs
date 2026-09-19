namespace Hub.Application.Features.Payments.Messages;

public sealed record PaymentCancelled(
    Guid EventId,
    Guid PaymentId,
    string Purpose,
    Guid ReferenceId,
    DateTimeOffset OccurredOn
);