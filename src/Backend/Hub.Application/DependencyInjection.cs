using FluentValidation;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Queries.Get;
using Hub.Application.Features.Events.Queries.GetById;
using Hub.Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateEventCommand>(
            includeInternalTypes: true);
        
        services.AddDecoratedHandler<CreateEventCommand, EventSummaryResponse, CreateEventCommandHandler>();
        services.AddDecoratedHandler<GetEventQuery, ListResponse<EventSummaryResponse>, GetEventQueryHandler>();
        services.AddDecoratedHandler<GetEventByIdQuery, EventDetailsResponse, GetEventByIdQueryHandler>();
    }
}