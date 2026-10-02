namespace Hub.Application.Features.Payments.Queries.GetInvoiceById;

public sealed record GetInvoiceByIdQuery(
    Guid UserId,
    Guid InvoiceId
);