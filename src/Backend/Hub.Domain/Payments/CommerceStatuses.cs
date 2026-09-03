namespace Hub.Domain.Payments;

public enum DonationStatus
{
    Created = 0,
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum SubscriptionStatus
{
    PendingPayment = 0,
    Active = 1,
    Paused = 2,
    Cancelled = 3
}

public enum BillingInterval
{
    Monthly = 0,
    Yearly = 1
}

public enum InvoiceStatus
{
    Draft = 0,
    Open = 1,
    Paid = 2,
    Overdue = 3,
    Void = 4
}

public enum OrderStatus
{
    Created = 0,
    AwaitingPayment = 1,
    Paid = 2,
    Completed = 3,
    Cancelled = 4
}

public enum WebhookProcessingStatus
{
    Received = 0,
    Processed = 1,
    Failed = 2
}
