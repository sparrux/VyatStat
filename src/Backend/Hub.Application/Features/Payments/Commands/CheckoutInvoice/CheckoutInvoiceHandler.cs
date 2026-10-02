using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Hub.Domain.Payments;

namespace Hub.Application.Features.Payments.Commands.CheckoutInvoice;

sealed class CheckoutInvoiceHandler(
    IPaymentsDbContext paymentsDbContext,
    IInvoiceService invoiceService,
    IPaymentService paymentService
) : IRequestHandler<CheckoutInvoiceCommand, CheckoutInvoiceResponse>
{
    public async Task<Result<CheckoutInvoiceResponse>> Handle(
        CheckoutInvoiceCommand command, 
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        
        var invoice = await invoiceService.FindInvoiceAsync(
            request.CustomerId,
            command.InvoiceId, 
            cancellationToken);
        if (!invoice.IsSuccess)
            return invoice.Map();
        
        var invoiceValue = invoice.Value;
        
        Uri? approvalUrl = null;
        Payment? paymentValue;

        if (invoiceValue.PaymentId is null && IsPendingForPayment(invoiceValue))
        {
            var paymentCreation = await paymentService.CreatePaymentAsync(
                new CreatePaymentRequest(
                    request.CustomerId,
                    invoiceValue.Amount.Amount,
                    invoiceValue.Amount.Currency.Code,
                    invoiceValue.Id,
                    PaymentPurpose.Invoice,
                    request.IdempotencyKey,
                    IsAnonymous: false
                ),
                cancellationToken
            );

            if (!paymentCreation.IsSuccess)
                return paymentCreation.Map();
            
            paymentValue = paymentCreation.Value.Payment;
            
            var attachPayment = invoiceValue.AttachPayment(paymentValue.Id);
            if (!attachPayment.IsSuccess)
                return attachPayment.Map();

            await paymentsDbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var paymentId = invoiceValue.PaymentId ?? Guid.Empty;
            
            var payment = await paymentService.FindPaymentAsync(
                paymentId,
                cancellationToken);
            if (!payment.IsSuccess)
                return payment.Map();

            if (payment.Value.Purpose != PaymentPurpose.Invoice)
                return Result.Error("Payment has difference purpose for checkout, not an Invoice");
            
            paymentValue = payment.Value;
        }

        if (IsPendingForPayment(invoiceValue))
        {
            var checkoutResult = await paymentService.StartCheckoutAsync(
                paymentValue, 
                new PaymentCheckoutRequest(
                    invoiceValue.Id,
                    request.IdempotencyKey,
                    request.PaymentProvider,
                    request.Description,
                    ReturnUrl: null,
                    CancelUrl: null
                ), cancellationToken);
            if (!checkoutResult.IsSuccess)
                return checkoutResult.Map();
            
            paymentValue = checkoutResult.Value.Payment;
            approvalUrl = checkoutResult.Value.ApprovalUrl;
        }

        return CheckoutInvoiceResponse.From(
            invoiceValue, 
            paymentValue, 
            approvalUrl);
    }

    static bool IsPendingForPayment(Invoice invoice)
    {
        return invoice.Status is InvoiceStatus.Open or InvoiceStatus.Overdue;
    }
}