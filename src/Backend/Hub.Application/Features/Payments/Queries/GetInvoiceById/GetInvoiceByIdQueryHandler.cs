using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Queries.GetInvoiceById;

sealed class GetInvoiceByIdQueryHandler(
    IPaymentsDbContext paymentsDbContext
) : IRequestHandler<GetInvoiceByIdQuery, InvoiceResponse>
{
    public async Task<Result<InvoiceResponse>> Handle(
        GetInvoiceByIdQuery query,
        CancellationToken cancellationToken)
    {
        var invoice = await paymentsDbContext.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == query.InvoiceId && x.CustomerId == query.UserId,
                cancellationToken);

        if (invoice is null)
            return Result.NotFound("Invoice not found");

        return Result.Success(InvoiceResponse.From(invoice));
    }
}