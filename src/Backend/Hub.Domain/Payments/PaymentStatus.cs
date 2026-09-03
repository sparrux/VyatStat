namespace Hub.Domain.Payments;

public enum PaymentStatus
{
    Created = 0,
    Pending = 1,
    RequiresAction = 2,
    Processing = 3,
    Succeeded = 4,
    Failed = 5,
    Cancelled = 6
}

public enum PaymentPurpose
{
    Donation = 0,
    Invoice = 1,
    Order = 2
}

public enum PaymentAttemptStatus
{
    Pending = 0,
    RequiresAction = 1,
    Processing = 2,
    Succeeded = 3,
    Failed = 4,
    Cancelled = 5
}

public enum RefundStatus
{
    Pending = 0,
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    Cancelled = 4
}
