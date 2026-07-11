using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Queries.Get;
using Hub.Application.Features.Events.Queries.GetById;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class EventsEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var events = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/events")
            .RequireAuthorization();

        events.MapPost("/", Create)
            .HasApiVersion(1.0)
            .Produces<EventSummaryResponse>(StatusCodes.Status201Created);
        
        events.MapGet("/", Get)
            .HasApiVersion(1.0)
            .Produces<EventDetailsResponse>();
        
        events.MapGet("/{eventId:guid}", GetById)
            .HasApiVersion(1.0)
            .Produces<ListResponse<EventSummaryResponse>>();
    }
    
    static async Task<IResult> Create(
        [FromBody] CreateEventCommand request,
        [FromServices] IRequestHandler<CreateEventCommand, EventSummaryResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(request, ctk)).ToMinimalApiResult();
    
    static async Task<IResult> Get(
        [AsParameters] GetEventQuery query,
        [FromServices] IRequestHandler<GetEventQuery, ListResponse<EventSummaryResponse>> handler,
        CancellationToken ctk) =>
        (await handler.Handle(query, ctk)).ToMinimalApiResult();
    
    static async Task<IResult> GetById(
        [FromRoute] Guid eventId,
        [FromServices] IRequestHandler<GetEventByIdQuery, EventDetailsResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId), ctk)).ToMinimalApiResult();
}