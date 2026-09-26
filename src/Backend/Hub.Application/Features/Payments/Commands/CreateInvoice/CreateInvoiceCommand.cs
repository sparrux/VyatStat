using Hub.Application.Features.Common.Contracts;
using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Commands.CreateInvoice;

public sealed record CreateInvoiceRequest(
    Guid CustomerId,
    decimal Amount,
    string Currency,
    DateTimeOffset DueDate,
    DatesRangeModel? BillingPeriod,
    Guid? SourceId,
    InvoiceSourceType SourceType = InvoiceSourceType.None
);

public sealed record CreateInvoiceCommand(
    CreateInvoiceRequest Request
);