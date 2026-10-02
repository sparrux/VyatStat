using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
public sealed class PaymentMethod : Auditable
{
    PaymentMethod() { }

    PaymentMethod(
        Customer customer,
        ProviderName provider,
        string providerPaymentMethodId,
        PaymentMethodType type,
        string? brand,
        string? lastFour)
    {
        Id = Guid.NewGuid();
        Customer = customer;
        CustomerId = customer.Id;
        Provider = provider;
        ProviderPaymentMethodId = providerPaymentMethodId;
        Type = type;
        Brand = brand;
        LastFour = lastFour;
    }

    public Customer Customer { get; private set; }
    public Guid CustomerId { get; private set; }

    public ProviderName Provider { get; private set; }
    public string ProviderPaymentMethodId { get; private set; }
    public PaymentMethodType Type { get; private set; }
    public string? Brand { get; private set; }
    public string? LastFour { get; private set; }
    public bool IsDefault { get; private set; }

    internal static Result<PaymentMethod> Create(
        Customer customer,
        ProviderName provider,
        string providerPaymentMethodId,
        PaymentMethodType type,
        string? brand,
        string? lastFour)
    {
        if (string.IsNullOrWhiteSpace(providerPaymentMethodId))
            return Result.Invalid(new ValidationError("Provider payment method id cannot be null or whitespace"));

        if (!Enum.IsDefined(type))
            return Result.Invalid(new ValidationError("Payment method type is not defined"));

        if (lastFour is not null && (lastFour.Length != 4 || !lastFour.All(char.IsDigit)))
            return Result.Invalid(new ValidationError("Last four digits must be exactly four digits"));

        return Result.Success(new PaymentMethod(
            customer,
            provider,
            providerPaymentMethodId.Trim(),
            type,
            string.IsNullOrWhiteSpace(brand) ? null : brand.Trim(),
            lastFour));
    }

    internal void SetDefault(bool isDefault) => IsDefault = isDefault;
}
