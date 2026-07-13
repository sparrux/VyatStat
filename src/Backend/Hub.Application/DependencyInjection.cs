using FluentValidation;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Commands.CreateInvitee;
using Hub.Application.Features.Events.Commands.CreateOrganizer;
using Hub.Application.Features.Events.Commands.CreateRequirement;
using Hub.Application.Features.Events.Commands.DeleteDescription;
using Hub.Application.Features.Events.Commands.DeleteLocation;
using Hub.Application.Features.Events.Commands.DeleteOrganizer;
using Hub.Application.Features.Events.Commands.DeleteRequirement;
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
using Hub.Application.Features.Events.Queries.GetInviteeById;
using Hub.Application.Features.Users.Contracts;
using Hub.Application.Features.Users.Queries.Get;
using Hub.Application.Features.Users.Queries.GetById;
using Hub.Application.Pipelines;
using Microsoft.Extensions.DependencyInjection;

namespace Hub.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplication()
        {
            services.AddValidatorsFromAssemblyContaining<CreateEventRequest>(
                includeInternalTypes: true);
        
            services.AddUserHandlers();
            services.AddEventHandlers();
        }

        void AddUserHandlers()
        {
            services.AddDecoratedHandler<GetUserQuery, ListResponse<UserSummaryResponse>, GetUserQueryHandler>();
            services.AddDecoratedHandler<GetUserByIdQuery, UserDetailsResponse, GetUserByIdQueryHandler>();
        }

        void AddEventHandlers()
        {
            services.AddDecoratedHandler<CreateEventCommand, EventSummaryResponse, CreateEventCommandHandler>();
            services.AddDecoratedHandler<GetEventQuery, ListResponse<EventSummaryResponse>, GetEventQueryHandler>();
            services.AddDecoratedHandler<GetEventByIdQuery, EventDetailsResponse, GetEventByIdQueryHandler>();
            services.AddDecoratedHandler<UpdateTitleCommand, IdResponse, UpdateTitleCommandHandler>();
            services.AddDecoratedHandler<UpdateDescriptionCommand, IdResponse, UpdateDescriptionCommandHandler>();
            services.AddDecoratedHandler<DeleteDescriptionCommand, IdResponse, DeleteDescriptionCommandHandler>();
            services.AddDecoratedHandler<UpdateDatesCommand, IdResponse, UpdateDatesCommandHandler>();
            services.AddDecoratedHandler<UpdateLocationCommand, IdResponse, UpdateLocationCommandHandler>();
            services.AddDecoratedHandler<DeleteLocationCommand, IdResponse, DeleteLocationCommandHandler>();
            services.AddDecoratedHandler<CreateInviteeCommand, EventInviteeSummaryResponse, CreateInviteeCommandHandler>();
            services.AddDecoratedHandler<GetInviteeByIdQuery, EventInviteeDetailsResponse, GetInviteeByIdQueryHandler>();
            services.AddDecoratedHandler<CreateOrganizerCommand, EventOrganizerSummaryResponse, CreateOrganizerCommandHandler>();
            services.AddDecoratedHandler<DeleteOrganizerCommand, IdResponse, DeleteOrganizerCommandHandler>();
            services.AddDecoratedHandler<UpdateStateCommand, IdResponse, UpdateStateCommandHandler>();
            services.AddDecoratedHandler<CreateRequirementCommand, EventRequirementSummaryResponse, CreateRequirementCommandHandler>();
            services.AddDecoratedHandler<UpdateRequirementCommand, IdResponse, UpdateRequirementCommandHandler>();
            services.AddDecoratedHandler<DeleteRequirementCommand, IdResponse, DeleteRequirementCommandHandler>();
            services.AddDecoratedHandler<UpdateCompletionCommand, IdResponse, UpdateCompletionCommandHandler>();
        }
    }
}