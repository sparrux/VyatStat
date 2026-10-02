using Ardalis.Result;

namespace Hub.Infrastructure.Payments.Options;

sealed class PayPalOptions
{
    public const string SectionName = "Payments:PayPal";

    public const string SandboxApiBase = "https://api-m.sandbox.paypal.com/";
    public const string LiveApiBase = "https://api-m.paypal.com/";

    public bool Enabled { get; init; }
    public string Environment { get; init; } = "Sandbox";
    public string ClientId { get; init; } = "";
    public string ClientSecret { get; init; } = "";
    public string? WebhookId { get; init; }
    public string? BrandName { get; init; }
    public Uri? ReturnUrl { get; init; }
    public Uri? CancelUrl { get; init; }

    public bool IsLive =>
        string.Equals(Environment, "Live", StringComparison.OrdinalIgnoreCase);

    public Uri ApiBaseUri => new(IsLive ? LiveApiBase : SandboxApiBase);

    public Result EnsureConfigured()
    {
        if (!Enabled)
            return Result.Error("PayPal gateway is disabled");

        if (string.IsNullOrWhiteSpace(ClientId))
            return Result.Error("Payments:PayPal:ClientId is not configured");

        if (string.IsNullOrWhiteSpace(ClientSecret))
            return Result.Error("Payments:PayPal:ClientSecret is not configured");

        return Result.Success();
    }
}
