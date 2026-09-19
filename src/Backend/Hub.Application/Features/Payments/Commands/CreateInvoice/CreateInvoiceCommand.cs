using Hub.Application.Features.Common.Contracts;

namespace Hub.Application.Features.Payments.Commands.CreateInvoice;

public sealed record CreateInvoiceRequest(
    Guid CustomerId,
    decimal Amount,
    string Currency,
    DateTimeOffset DueDate,
    DatesRangeModel? BillingPeriod
);

public sealed record CreateInvoiceCommand(
    CreateInvoiceRequest Request
);