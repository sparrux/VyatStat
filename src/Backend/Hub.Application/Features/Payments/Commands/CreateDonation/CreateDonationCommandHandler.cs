using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;
using Hub.Domain.Payments.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Commands.CreateDonation;

sealed class CreateDonationCommandHandler(
    IPaymentsDbContext paymentsDbContext,
    IPaymentService paymentService
) : IRequestHandler<CreateDonationCommand, DonationResponse>
{
    public async Task<Result<DonationResponse>> Handle(
        CreateDonationCommand command,
        CancellationToken cancellationToken)
    {
        PaymentCheckoutRequest checkoutRequest;
        Result<PaymentCheckoutResult> checkoutResult;
        
        var request = command.Request;
        
        var foundByIdempotency = await FindByIdempotencyKeyAsync(request.IdempotencyKey, cancellationToken);

        if (foundByIdempotency is var (dbDonation, dbPayment))
        {
            checkoutRequest = CreateCheckoutRequest(command, dbDonation);
            
            checkoutResult = await paymentService.StartCheckoutAsync(
                dbPayment, 
                checkoutRequest, 
                cancellationToken);

            if (!checkoutResult.IsSuccess)
                return checkoutResult.Map();

            var checkoutValue = checkoutResult.Value;
            return DonationResponse.From(dbDonation, checkoutValue.Payment, checkoutValue.ApprovalUrl);
        }
        
        var amount = Money.Create(request.Amount, request.Currency);
        if (!amount.IsSuccess)
            return amount.Map();
        
        var donation = Donation.Create(amount.Value, command.UserId, request.IsAnonymous, reference: null);
        if (!donation.IsSuccess)
            return donation.Map();
        
        await paymentsDbContext.Donations.AddAsync(donation.Value, cancellationToken);

        checkoutRequest = CreateCheckoutRequest(command, donation);
        var paymentRequest = CreatePaymentRequest(command, donation);

        var paymentResult = await paymentService.CreatePaymentAsync(paymentRequest, cancellationToken);
        
        if (!paymentResult.IsSuccess)
            return paymentResult.Map();

        var payment = paymentResult.Value.Payment;

        var attached = donation.Value.AttachPayment(payment.Id);
        if (!attached.IsSuccess)
            return attached.Map();

        await paymentsDbContext.SaveChangesAsync(cancellationToken);

        checkoutResult = await paymentService.StartCheckoutAsync(payment, checkoutRequest, cancellationToken);

        return DonationResponse.From(donation, payment, checkoutResult.Value.ApprovalUrl);
    }
    
    async Task<(Donation Donation, Payment Payment)?> FindByIdempotencyKeyAsync(string idempotencyKey, CancellationToken cancellationToken)
    {
        // TODO: Find returns NotFound Error if payment not found. But it should be handled by another way
        
        
        var payment = await paymentService.FindPaymentAsync(null, idempotencyKey, cancellationToken);
        
        if (!payment.IsSuccess)
            return null;
        
        var donation = await paymentsDbContext.Donations
            .FirstOrDefaultAsync(x => x.PaymentId == payment.Value.Id, cancellationToken);
        
        if (donation is null)
            return null;

        return (donation, payment);
    }
    
    static CreatePaymentRequest CreatePaymentRequest(CreateDonationCommand command, Donation donation)
    {
        var request = command.Request;
        
        return new CreatePaymentRequest(
            command.UserId,
            request.Amount,
            request.Currency,
            donation.Id,
            PaymentPurpose.Donation,
            request.IdempotencyKey,
            request.IsAnonymous
        );
    }

    static PaymentCheckoutRequest CreateCheckoutRequest(CreateDonationCommand command, Donation donation)
    {
        var request = command.Request;
        
        return new PaymentCheckoutRequest(
            donation.Id,
            request.IdempotencyKey,
            request.Provider,
            request.Description,
            request.ReturnUrl,
            request.CancelUrl
        );
    }
}
