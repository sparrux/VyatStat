using Ardalis.Result;
using Hub.Application.Abstractions.Payments;
using Hub.Infrastructure.Payments.Options;
using Microsoft.Extensions.Options;

namespace Hub.Infrastructure.Payments.Gateways;

sealed class PaymentGatewayResolver : IPaymentGatewayResolver
{
    readonly IReadOnlyDictionary<string, IPaymentGateway> _gateways;
    readonly string _defaultGateway;

    public PaymentGatewayResolver(
        IEnumerable<IPaymentGateway> gateways,
        IOptions<PaymentsOptions> options)
    {
        _gateways = gateways.ToDictionary(
            gateway => gateway.Name,
            gateway => gateway,
            StringComparer.OrdinalIgnoreCase);

        _defaultGateway = string.IsNullOrWhiteSpace(options.Value.DefaultGateway)
            ? PaymentGatewayNames.PayPal
            : options.Value.DefaultGateway;
    }

    public Result<IPaymentGateway> Resolve(string? providerName = null)
    {
        var name = string.IsNullOrWhiteSpace(providerName)
            ? _defaultGateway
            : providerName.Trim();

        if (_gateways.TryGetValue(name, out var gateway))
            return Result.Success(gateway);

        var registered = string.Join(", ", _gateways.Keys);
        return Result.NotFound(
            $"Payment gateway '{name}' is not registered. Available: {registered}");
    }
}
