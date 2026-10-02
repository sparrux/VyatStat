namespace Hub.Application.Features.Payments.Commands.VoidInvoice;

public sealed record VoidInvoiceCommand(
    Guid InvoiceId
);