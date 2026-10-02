namespace Hub.Application.Features.Payments.Commands.CheckoutInvoice;

public sealed record CheckoutInvoiceRequest(
    Guid CustomerId,
    string IdempotencyKey,
    string PaymentProvider,
    string Description
);

public sealed record CheckoutInvoiceCommand(
    Guid InvoiceId,
    CheckoutInvoiceRequest Request
);