using Ardalis.Result;

namespace Hub.Application.Abstractions.Payments;

public interface IPaymentGatewayResolver
{
    Result<IPaymentGateway> Resolve(string? providerName = null);
}
