using FluentValidation;
using Hub.Application.Features.Common.Contracts;
using Hub.Application.Features.Events.Commands.Create;
using Hub.Application.Features.Events.Commands.CreateParticipant;
using Hub.Application.Features.Events.Commands.DeleteDescription;
using Hub.Application.Features.Events.Commands.DeleteLocation;
using Hub.Application.Features.Events.Commands.DeleteRequirement;
using Hub.Application.Features.Events.Commands.UpdateCompletion;
using Hub.Application.Features.Events.Commands.UpdateDates;
using Hub.Application.Features.Events.Commands.UpdateDescription;
using Hub.Application.Features.Events.Commands.UpdateLocation;
using Hub.Application.Features.Events.Commands.UpdateState;
using Hub.Application.Features.Events.Commands.UpdateTitle;
using Hub.Application.Features.Events.Contracts;
using Hub.Application.Features.Events.Queries.Get;
using Hub.Application.Features.Events.Queries.GetById;
using Hub.Application.Features.Events.Queries.GetParticipantById;
using Hub.Application.Features.Groups.Commands.AttachEvent;
using Hub.Application.Features.Groups.Commands.Create;
using Hub.Application.Features.Groups.Contracts;
using Hub.Application.Features.Groups.Queries.Get;
using Hub.Application.Features.Groups.Queries.GetEvents;
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
            services.AddGroupHandlers();
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
            services.AddDecoratedHandler<CreateParticipantCommand, EventParticipantSummaryResponse, CreateParticipantCommandHandler>();
            services.AddDecoratedHandler<GetParticipantByIdQuery, EventParticipantDetailsResponse, GetParticipantByIdQueryHandler>();
            services.AddDecoratedHandler<UpdateStateCommand, IdResponse, UpdateStateCommandHandler>();
            services.AddDecoratedHandler<DeleteRequirementCommand, IdResponse, DeleteRequirementCommandHandler>();
            services.AddDecoratedHandler<UpdateCompletionCommand, IdResponse, UpdateCompletionCommandHandler>();
        }

        void AddGroupHandlers()
        {
            services.AddDecoratedHandler<CreateGroupCommand, GroupSummaryResponse, CreateGroupCommandHandler>();
            services.AddDecoratedHandler<GetGroupQuery, ListResponse<GroupSummaryResponse>, GetGroupQueryHandler>();
            services.AddDecoratedHandler<GetGroupEventsQuery, ListResponse<EventSummaryResponse>, GetGroupEventsQueryHandler>();
            services.AddDecoratedHandler<AttachEventCommand, IdResponse, AttachEventCommandHandler>();
        }
    }
}