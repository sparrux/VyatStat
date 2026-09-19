using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Pipelines;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Commands.VoidInvoice;

sealed class VoidInvoiceCommandHandler(
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
        
        await paymentsDbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success(new IdResponse(invoice.Id));
    }
}