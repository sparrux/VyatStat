using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hub.Infrastructure.Payments.Gateways.PayPal;

static class PayPalJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true
    };
}

sealed record PayPalTokenResponse
{
    public string? AccessToken { get; init; }
    public int ExpiresIn { get; init; }
}

sealed record PayPalOrderRequest
{
    public required string Intent { get; init; }
    public required IReadOnlyList<PayPalPurchaseUnit> PurchaseUnits { get; init; }
    public PayPalPaymentSource? PaymentSource { get; init; }
}

sealed record PayPalPurchaseUnit
{
    public string? ReferenceId { get; init; }
    public string? CustomId { get; init; }
    public string? Description { get; init; }
    public required PayPalAmount Amount { get; init; }
    public PayPalPayments? Payments { get; init; }
}

sealed record PayPalAmount
{
    public required string CurrencyCode { get; init; }
    public required string Value { get; init; }
}

sealed record PayPalPaymentSource
{
    public PayPalWallet? Paypal { get; init; }
}

sealed record PayPalWallet
{
    public PayPalExperienceContext? ExperienceContext { get; init; }
}

sealed record PayPalExperienceContext
{
    public string? BrandName { get; init; }
    public string? ShippingPreference { get; init; }
    public string? UserAction { get; init; }
    public string? ReturnUrl { get; init; }
    public string? CancelUrl { get; init; }
}

sealed record PayPalOrderResponse
{
    public string? Id { get; init; }
    public string? Status { get; init; }
    public IReadOnlyList<PayPalLink>? Links { get; init; }
    public IReadOnlyList<PayPalPurchaseUnit>? PurchaseUnits { get; init; }
    public string? Name { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<PayPalErrorDetail>? Details { get; init; }
}

sealed record PayPalLink
{
    public string? Href { get; init; }
    public string? Rel { get; init; }
}

sealed record PayPalPayments
{
    public IReadOnlyList<PayPalCapture>? Captures { get; init; }
}

sealed record PayPalCapture
{
    public string? Id { get; init; }
    public string? Status { get; init; }
}

sealed record PayPalRefundRequest
{
    public required PayPalAmount Amount { get; init; }
}

sealed record PayPalRefundResponse
{
    public string? Id { get; init; }
    public string? Status { get; init; }
    public string? Name { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<PayPalErrorDetail>? Details { get; init; }
}

sealed record PayPalErrorDetail
{
    public string? Issue { get; init; }
    public string? Description { get; init; }
}
