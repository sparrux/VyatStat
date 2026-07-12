using FluentValidation;
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
        
        services.AddDecoratedHandler<UpdateTitleCommand, IdResponse, UpdateTitleCommandHandler>();
        services.AddDecoratedHandler<UpdateDescriptionCommand, IdResponse, UpdateDescriptionCommandHandler>();
        services.AddDecoratedHandler<DeleteDescriptionCommand, IdResponse, DeleteDescriptionCommandHandler>();
        services.AddDecoratedHandler<UpdateDatesCommand, IdResponse, UpdateDatesCommandHandler>();
        services.AddDecoratedHandler<UpdateLocationCommand, IdResponse, UpdateLocationCommandHandler>();
        services.AddDecoratedHandler<DeleteLocationCommand, IdResponse, DeleteLocationCommandHandler>();
    }
}