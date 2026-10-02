using Ardalis.Result;
using Hub.Application.Abstractions.Payments;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;

namespace Hub.Application.Features.Payments.Commands.CreateInvoice;

sealed class CreateInvoiceCommandHandler(
    IInvoiceService invoiceService
) : IRequestHandler<CreateInvoiceCommand, InvoiceResponse>
{
    public async Task<Result<InvoiceResponse>> Handle(CreateInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await invoiceService.CreateInvoiceAsync(
            command.Request, 
            cancellationToken);
        if (!invoice.IsSuccess)
            return invoice.Map();
        
        return Result.Success(InvoiceResponse.From(invoice.Value));
    }
}