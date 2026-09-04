using Hub.Application.Abstractions.Payments;

namespace Hub.Infrastructure.Payments.Options;

sealed class PaymentsOptions
{
    public const string SectionName = "Payments";

    public string DefaultGateway { get; init; } = PaymentGatewayNames.PayPal;
}
