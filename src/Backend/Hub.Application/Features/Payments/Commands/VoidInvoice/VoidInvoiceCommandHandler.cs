using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Commands.VoidInvoice;

sealed class VoidInvoiceCommandHandler(
    IPaymentService paymentService,
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<VoidInvoiceCommand, IdResponse>
{
    public async Task<Result<IdResponse>> Handle(VoidInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await paymentsDbContext.Invoices
            .FirstOrDefaultAsync(x => x.Id == command.InvoiceId, cancellationToken);
        
        if (invoice is null) return Result.NotFound("Invoice not found");

        var voidResult = invoice.Void();
        if (!voidResult.IsSuccess) return voidResult.Map();

        if (invoice.PaymentId is { } paymentId)
        {
            var payment = await paymentService.FindPaymentAsync(paymentId, cancellationToken);
            if (!payment.IsSuccess) return Result.NotFound("Payment not found");

            var cancellation = payment.Value.Cancel();
            if (!cancellation.IsSuccess) return cancellation.Map();
        }
        
        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(invoice.Id));
    }
}