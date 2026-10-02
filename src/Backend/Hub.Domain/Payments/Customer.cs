using System.Diagnostics.CodeAnalysis;
using Ardalis.Result;
using Hub.Domain.Common;
using Hub.Domain.Payments.ValueObjects;

#pragma warning disable CS8618

namespace Hub.Domain.Payments;

[SuppressMessage("ReSharper", "UnusedMember.Local")]
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Local")]
public sealed class Customer : AggregateRoot
{
    readonly List<PaymentMethod> _paymentMethods = [];

    Customer() { }

    Customer(Guid id)
    {
        Id = id;
    }

    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods;

    public static Result<Customer> Create(Guid id)
    {
        if (id == Guid.Empty)
            return Result.Invalid(new ValidationError("Customer id cannot be empty"));

        return Result.Success(new Customer(id));
    }

    public Result<PaymentMethod> AddPaymentMethod(
        ProviderName provider,
        string providerPaymentMethodId,
        PaymentMethodType type,
        string? brand = null,
        string? lastFour = null)
    {
        if (string.IsNullOrWhiteSpace(providerPaymentMethodId))
            return Result.Invalid(new ValidationError("Provider payment method id cannot be null or whitespace"));

        var providerMethodId = providerPaymentMethodId.Trim();
        if (_paymentMethods.Any(x =>
                x.Provider == provider &&
                string.Equals(x.ProviderPaymentMethodId, providerMethodId, StringComparison.Ordinal)))
        {
            return Result.Error("Payment method already exists for this provider");
        }

        var method = PaymentMethod.Create(this, provider, providerMethodId, type, brand, lastFour);
        if (!method.IsSuccess)
            return method;

        if (_paymentMethods.Count == 0)
            method.Value.SetDefault(true);

        _paymentMethods.Add(method.Value);
        return method;
    }

    public Result SetDefaultPaymentMethod(Guid paymentMethodId)
    {
        var method = _paymentMethods.FirstOrDefault(x => x.Id == paymentMethodId);
        if (method is null)
            return Result.NotFound("Payment method not found");

        foreach (var existing in _paymentMethods)
            existing.SetDefault(existing.Id == paymentMethodId);

        return Result.Success();
    }

    public Result RemovePaymentMethod(Guid paymentMethodId)
    {
        var method = _paymentMethods.FirstOrDefault(x => x.Id == paymentMethodId);
        if (method is null)
            return Result.NotFound("Payment method not found");

        _paymentMethods.Remove(method);

        if (method.IsDefault)
        {
            var next = _paymentMethods.FirstOrDefault();
            next?.SetDefault(true);
        }

        return Result.Success();
    }
}
