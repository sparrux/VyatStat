using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Payments.Commands.CreateInvoice;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Features.Payments.Queries.GetInvoices;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        var invoices = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/invoices")
            .RequireAuthorization();

        invoices.MapPost("/", Create)
            .HasApiVersion(1.0)
            .Produces<InvoiceResponse>(StatusCodes.Status201Created);
        
        invoices.MapGet("/", Get)
            .HasApiVersion(1.0)
            .Produces<ListResponse<InvoiceResponse>>();
    }

    static async Task<IResult> Create(
        [FromBody] CreateInvoiceRequest request,
        [FromServices] IRequestHandler<CreateInvoiceCommand, InvoiceResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(request), ctk))
        .ToMinimalApiResult();
    
    static async Task<IResult> Get(
        [AsParameters] GetInvoicesQuery query,
        [FromServices] IRequestHandler<GetInvoicesQuery, ListResponse<InvoiceResponse>> handler,
        CancellationToken ctk) =>
        (await handler.Handle(query, ctk))
        .ToMinimalApiResult();
}