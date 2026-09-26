using Hub.Application.Features.Common.Contracts;
using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record InvoiceResponse(
    Guid Id,
    MoneyResponse Amount,
    DateTimeOffset DueDate,
    DatesRangeModel? BillingPeriod,
    Guid? SourceId,
    InvoiceSourceType SourceType,
    InvoiceStatus Status
)
{
    public static InvoiceResponse From(Invoice invoice) =>
        new(
            invoice.Id,
            new MoneyResponse(invoice.Amount.Amount, invoice.Amount.Currency.Code),
            invoice.DueDate,
            invoice.BillingPeriod is null
                ? null
                : new DatesRangeModel(invoice.BillingPeriod.StartDate, invoice.BillingPeriod.EndDate),
            invoice.SourceId,
            invoice.SourceType,
            invoice.Status
        );
}