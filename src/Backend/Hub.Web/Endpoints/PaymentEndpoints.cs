using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Payments.Commands.CancelCash;
using Hub.Application.Features.Payments.Commands.ConfirmCash;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this WebApplication app)
    {
        var payments = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/payments")
            .RequireAuthorization();
        
        var cashProvider = payments.MapGroup("/cash-provider");

        // donations.MapPost("/cash", RecordCash)
        //     .HasApiVersion(1.0)
        //     .Produces<DonationResponse>(StatusCodes.Status201Created);
        //
        // donations.MapGet("/{donationId:guid}", GetById)
        //     .HasApiVersion(1.0)
        //     .Produces<DonationResponse>();

        cashProvider.MapPost("/{paymentId:guid}/confirm", ConfirmCash)
            .HasApiVersion(1.0)
            .Produces<PaymentResponse>();

        cashProvider.MapPost("/{paymentId:guid}/cancel", CancelCash)
            .HasApiVersion(1.0)
            .Produces<PaymentResponse>();
    }

    // static async Task<IResult> RecordCash(
    //     [FromBody] RecordCashDonationRequest request,
    //     [FromServices] IRequestHandler<RecordCashDonationCommand, DonationResponse> handler,
    //     CancellationToken ctk) =>
    //     (await handler.Handle(new(request), ctk))
    //     .ToMinimalApiResult();
    //
    // static async Task<IResult> GetById(
    //     [FromRoute] Guid donationId,
    //     [FromServices] IUserContext userContext,
    //     [FromServices] IRequestHandler<GetDonationByIdQuery, DonationResponse> handler,
    //     CancellationToken ctk) =>
    //     (await handler.Handle(new(userContext.UserId, donationId), ctk))
    //     .ToMinimalApiResult();

    static async Task<IResult> ConfirmCash(
        [FromRoute] Guid paymentId,
        [FromServices] IRequestHandler<ConfirmCashCommand, PaymentResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(paymentId), ctk))
        .ToMinimalApiResult();

    static async Task<IResult> CancelCash(
        [FromRoute] Guid paymentId,
        [FromServices] IRequestHandler<CancelCashCommand, PaymentResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(paymentId), ctk))
        .ToMinimalApiResult();
}