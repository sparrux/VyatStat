namespace Hub.Application.Features.Payments.Commands.HandlePaymentCancelled;

public sealed record HandlePaymentCancelledCommand(
    Guid PaymentId,
    string Purpose,
    Guid ReferenceId
);