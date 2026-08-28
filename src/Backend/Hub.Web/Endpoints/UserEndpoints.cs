using Ardalis.Result.AspNetCore;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Features.Users.Queries.Get;
using Hub.Application.Features.Users.Queries.GetById;
using Hub.Application.Pipelines;
using Microsoft.AspNetCore.Mvc;

namespace Hub.Web.Endpoints;

static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var users = app.NewVersionedApi()
            .MapGroup("/api/v{version:apiVersion}/users")
            .RequireAuthorization();
        
        users.MapGet("/", Get)
            .HasApiVersion(1.0)
            .Produces<ListResponse<EventSummaryResponse>>();

        users.MapGet("/{userId:guid}", GetById)
            .HasApiVersion(1.0)
            .Produces<UserDetailsResponse>();
    }
    
    static async Task<IResult> Get(
        [AsParameters] GetUserQuery query,
        [FromServices] IRequestHandler<GetUserQuery, ListResponse<UserSummaryResponse>> handler,
        CancellationToken ctk) =>
        (await handler.Handle(query, ctk)).ToMinimalApiResult();
    
    static async Task<IResult> GetById(
        [FromRoute] Guid userId,
        [FromServices] IRequestHandler<GetUserByIdQuery, UserDetailsResponse> handler,
        CancellationToken ctk) =>
        (await handler.Handle(new(userId), ctk)).ToMinimalApiResult();
}