using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Hangfire;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Payments.Commands.CreateDonation;
using Hub.Application.Features.Payments.Contracts;
using Hub.Application.Features.Payments.Queries.GetDonationById;
using Hub.Application.Features.Payments.Queries.GetDonations;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Hub.Web.Endpoints;

static class DonationEndpoints
{
    public static void MapDonationEndpoints(this WebApplication app)
    {
        var donations = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/donations")
            .RequireAuthorization();

        donations.MapPost("/", Create)
            .HasApiVersion(1.0)
            .Produces<DonationResponse>(StatusCodes.Status201Created);
        
        donations.MapGet("/", Get)
            .HasApiVersion(1.0)
            .Produces<ListResponse<DonationResponse>>();

        donations.MapGet("/{donationId:guid}", GetById)
            .HasApiVersion(1.0)
            .Produces<DonationResponse>();
    }

    static async Task<IResult> Create(
        [FromBody] CreateDonationRequest request,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<CreateDonationCommand, DonationResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(userContext.UserId, request), ctk))
        .ToMinimalApiResult();
    
    static async Task<IResult> Get(
        [AsParameters] GetDonationsQuery query,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<GetDonationsQuery, ListResponse<DonationResponse>> handler,
        CancellationToken ctk)
    {
        if (userContext.UserId != query.UserId)
            return Result.Forbidden("Forbidden for receive another user's donations").ToMinimalApiResult();

        return (await handler.Handle(query, ctk))
            .ToMinimalApiResult();
    }

    static async Task<IResult> GetById(
        [FromRoute] Guid donationId,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<GetDonationByIdQuery, DonationResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(userContext.UserId, donationId), ctk))
        .ToMinimalApiResult();
}
