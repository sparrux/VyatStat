using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Commands.DeleteDescription;
using Hub.Application.Features.Events.Commands.DeleteLocation;
using Hub.Application.Features.Events.Commands.UpdateDates;
using Hub.Application.Features.Events.Commands.UpdateDescription;
using Hub.Application.Features.Events.Commands.UpdateLocation;
using Hub.Application.Features.Events.Commands.UpdateTitle;
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
        
        events.MapPut("/{eventId:guid}/title", UpdateTitle)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPut("/{eventId:guid}/description", UpdateDescription)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapDelete("/{eventId:guid}/description", DeleteDescription)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPut("/{eventId:guid}/dates", UpdateDates)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPut("/{eventId:guid}/location", UpdateLocation)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapDelete("/{eventId:guid}/location", DeleteLocation)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
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
    
    static async Task<IResult> UpdateTitle(
        [FromRoute] Guid eventId,
        [FromBody] UpdateTitleRequest request,
        [FromServices] IRequestHandler<UpdateTitleCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> UpdateDescription(
        [FromRoute] Guid eventId,
        [FromBody] UpdateDescriptionRequest request,
        [FromServices] IRequestHandler<UpdateDescriptionCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> DeleteDescription(
        [FromRoute] Guid eventId,
        [FromServices] IRequestHandler<DeleteDescriptionCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> UpdateDates(
        [FromRoute] Guid eventId,
        [FromBody] UpdateEventDatesRequest request,
        [FromServices] IRequestHandler<UpdateDatesCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> UpdateLocation(
        [FromRoute] Guid eventId,
        [FromBody] UpdateLocationRequest request,
        [FromServices] IRequestHandler<UpdateLocationCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> DeleteLocation(
        [FromRoute] Guid eventId,
        [FromServices] IRequestHandler<DeleteLocationCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId), ctk)).ToMinimalApiResult();
}