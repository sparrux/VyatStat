using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Contracts;

public sealed record DonationResponse(
    Guid Id,
    DonationStatus Status,
    bool IsAnonymous,
    PaymentResponse? Payment
)
{
    public static DonationResponse From(
        Donation donation,
        Payment? payment,
        Uri? approvalUrl = null) =>
        new(
            donation.Id,
            donation.Status,
            donation.IsAnonymous,
            payment is not null 
                ? PaymentResponse.From(payment, approvalUrl) 
                : null
        );
}
