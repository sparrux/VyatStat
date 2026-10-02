using Ardalis.Result;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Microsoft.EntityFrameworkCore;

namespace Hub.Application.Features.Payments.Queries.GetInvoices;

sealed class GetInvoicesQueryHandler(
    IPaymentsDbContext dbContext
) : IRequestHandler<GetInvoicesQuery, ListResponse<InvoiceResponse>>
{
    public async Task<Result<ListResponse<InvoiceResponse>>> Handle(
        GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoicesSelection = dbContext.Invoices
            .Where(i => i.CustomerId == request.UserId)
            .AsNoTracking();
        
        var invoices = await invoicesSelection
            .OrderByDescending(x => x.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        return new ListResponse<InvoiceResponse>(
            [.. invoices.Select(InvoiceResponse.From)],
            await invoicesSelection.CountAsync(cancellationToken));
    }
}