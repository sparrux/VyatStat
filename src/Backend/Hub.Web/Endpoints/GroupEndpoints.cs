using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Groups.Commands.Create;
using Hub.Application.Features.Groups.Contracts;
using Hub.Application.Features.Groups.Queries.Get;
using Hub.Application.Features.Groups.Queries.GetEvents;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class GroupEndpoints
{
    public static void MapGroupEndpoints(this WebApplication app)
    {
        var groups = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/groups")
            .RequireAuthorization();

        groups.MapPost("/", Create)
            .HasApiVersion(1.0)
            .Produces<GroupSummaryResponse>(StatusCodes.Status201Created);
        
        groups.MapGet("/", Get)
            .HasApiVersion(1.0)
            .Produces<ListResponse<GroupSummaryResponse>>();
        
        groups.MapGet("/events", GetEvents)
            .HasApiVersion(1.0)
            .Produces<ListResponse<EventSummaryResponse>>();
    }

    static async Task<IResult> Create(
        [FromBody] CreateGroupCommand request,
        [FromServices] IRequestHandler<CreateGroupCommand, GroupSummaryResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(request, ctk)).ToMinimalApiResult();

    static async Task<IResult> Get(
        [AsParameters] GetGroupQuery query,
        [FromServices] IRequestHandler<GetGroupQuery, ListResponse<GroupSummaryResponse>> handler,
        CancellationToken ctk) =>
        (await handler.Handle(query, ctk)).ToMinimalApiResult();
    
    static async Task<IResult> GetEvents(
        [AsParameters] GetGroupEventsQuery query,
        [FromServices] IRequestHandler<GetGroupEventsQuery, ListResponse<EventSummaryResponse>> handler,
        CancellationToken ctk) =>
        (await handler.Handle(query, ctk)).ToMinimalApiResult();
}