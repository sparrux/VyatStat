using Ardalis.Result.AspNetCore;
using Hub.Application.Abstractions;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Commands.CreateParticipant;
using Hub.Application.Features.Events.Commands.CreateParticipantRole;
using Hub.Application.Features.Events.Commands.CreateRequirement;
using Hub.Application.Features.Events.Commands.CreateRole;
using Hub.Application.Features.Events.Commands.DeleteDescription;
using Hub.Application.Features.Events.Commands.DeleteLocation;
using Hub.Application.Features.Events.Commands.DeleteParticipantRole;
using Hub.Application.Features.Events.Commands.DeleteRequirement;
using Hub.Application.Features.Events.Commands.DeleteRole;
using Hub.Application.Features.Events.Commands.UpdateCompletion;
using Hub.Application.Features.Events.Commands.UpdateDates;
using Hub.Application.Features.Events.Commands.UpdateDescription;
using Hub.Application.Features.Events.Commands.UpdateLocation;
using Hub.Application.Features.Events.Commands.UpdateRequirement;
using Hub.Application.Features.Events.Commands.UpdateState;
using Hub.Application.Features.Events.Commands.UpdateTitle;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Queries.Get;
using Hub.Application.Features.Events.Queries.GetById;
using Hub.Application.Features.Events.Queries.GetParticipantById;
using Hub.Application.Pipelines;
using Hub.Domain.Events;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class EventEndpoints
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
            .Produces<ListResponse<EventDetailsResponse>>();
        
        events.MapGet("/{eventId:guid}", GetById)
            .HasApiVersion(1.0)
            .Produces<EventDetailsResponse>();
        
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
        
        events.MapPost("/{eventId:guid}/participants", CreateParticipant)
            .HasApiVersion(1.0)
            .Produces<EventParticipantSummaryResponse>();
        
        events.MapGet("/{eventId:guid}/participants/me", GetParticipantBySelf)
            .HasApiVersion(1.0)
            .Produces<EventParticipantDetailsResponse>();
        
        events.MapGet("/{eventId:guid}/participants/{userId:guid}", GetParticipantByUserId)
            .HasApiVersion(1.0)
            .Produces<EventParticipantDetailsResponse>();
        
        events.MapPost("/{eventId:guid}/participants/{userId:guid}/roles/{roleId:guid}", CreateParticipantRole)
            .HasApiVersion(1.0)
            .Produces<EventParticipantRoleResponse>(StatusCodes.Status201Created);
        
        events.MapDelete("/{eventId:guid}/participants/{userId:guid}/roles/{roleId:guid}", DeleteParticipantRole)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPut("/{eventId:guid}/state", UpdateState)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPost("/{eventId:guid}/requirements", CreateRequirement)
            .HasApiVersion(1.0)
            .Produces<EventRequirementSummaryResponse>(StatusCodes.Status201Created);
        
        events.MapPut("/{eventId:guid}/requirements/{reqId:guid}", UpdateRequirement)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapDelete("/{eventId:guid}/requirements/{reqId:guid}", DeleteRequirement)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPost("/{eventId:guid}/roles", CreateRole)
            .HasApiVersion(1.0)
            .Produces<EventRoleSummaryResponse>(StatusCodes.Status201Created);
        
        events.MapDelete("/{eventId:guid}/roles/{roleId:guid}", DeleteRole)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
        
        events.MapPut("/{eventId:guid}/requirements/{reqId:guid}/completion/verify", UpdateCompletion)
            .HasApiVersion(1.0)
            .Produces<IdResponse>();
    }
    
    static async Task<IResult> Create(
        [FromBody] CreateEventRequest request,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<CreateEventCommand, EventSummaryResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(userContext.UserId, request), ctk)).ToMinimalApiResult();
    
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
    
    static async Task<IResult> CreateParticipant(
        [FromRoute] Guid eventId,
        [FromQuery] Guid? userId,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<CreateParticipantCommand, EventParticipantSummaryResponse> handler,
        CancellationToken ctk)
    {
        var participantUserId = userId ?? userContext.UserId;
        return (await handler.Handle(new(eventId, participantUserId), ctk)).ToMinimalApiResult();
    }
    
    static async Task<IResult> GetParticipantBySelf(
        [FromRoute] Guid eventId,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<GetParticipantByIdQuery, EventParticipantDetailsResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, userContext.UserId), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> GetParticipantByUserId(
        [FromRoute] Guid eventId,
        [FromRoute] Guid userId,
        [FromServices] IRequestHandler<GetParticipantByIdQuery, EventParticipantDetailsResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, userId), ctk)).ToMinimalApiResult();

    static async Task<IResult> CreateParticipantRole(
        [FromRoute] Guid eventId,
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        [FromServices] IRequestHandler<CreateParticipantRoleCommand, EventParticipantRoleResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, userId, roleId), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> DeleteParticipantRole(
        [FromRoute] Guid eventId,
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        [FromServices] IRequestHandler<DeleteParticipantRoleCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, userId, roleId), ctk)).ToMinimalApiResult();

    static async Task<IResult> UpdateState(
        [FromRoute] Guid eventId,
        [FromQuery] EventState state,
        [FromServices] IRequestHandler<UpdateStateCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, state), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> CreateRequirement(
        [FromRoute] Guid eventId,
        [FromBody] CreateRequirementRequest request,
        [FromServices] IRequestHandler<CreateRequirementCommand, EventRequirementSummaryResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> UpdateRequirement(
        [FromRoute] Guid eventId,
        [FromRoute] Guid reqId,
        [FromBody] UpdateRequirementRequest request,
        [FromServices] IRequestHandler<UpdateRequirementCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, reqId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> DeleteRequirement(
        [FromRoute] Guid eventId,
        [FromRoute] Guid reqId,
        [FromServices] IRequestHandler<DeleteRequirementCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, reqId), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> CreateRole(
        [FromRoute] Guid eventId,
        [FromBody] CreateRoleRequest request,
        [FromServices] IRequestHandler<CreateRoleCommand, EventRoleSummaryResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, request), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> DeleteRole(
        [FromRoute] Guid eventId,
        [FromRoute] Guid roleId,
        [FromServices] IRequestHandler<DeleteRoleCommand, IdResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(eventId, roleId), ctk)).ToMinimalApiResult();
    
    static async Task<IResult> UpdateCompletion(
        [FromRoute] Guid eventId,
        [FromRoute] Guid reqId,
        [FromQuery] Guid? userId,
        [FromServices] IUserContext userContext,
        [FromServices] IRequestHandler<UpdateCompletionCommand, IdResponse> handler,
        CancellationToken ctk)
    {
        var actor = userContext.UserId;
        var participantUserId = userId ?? userContext.UserId;
        return (await handler.Handle(
            new(eventId, participantUserId, reqId, actor), ctk)
        ).ToMinimalApiResult();
    }
}