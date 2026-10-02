using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record CheckoutInvoiceResponse(
    InvoiceResponse Invoice,
    PaymentResponse Payment
)
{
    public static CheckoutInvoiceResponse From(Invoice invoice, Payment payment, Uri? approvalUrl) => 
        new(InvoiceResponse.From(invoice), PaymentResponse.From(payment, approvalUrl));
};