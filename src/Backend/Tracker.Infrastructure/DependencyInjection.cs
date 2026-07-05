using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Interfaces.Events;
using Tracker.Application.Interfaces.Groups;
using Tracker.Application.Interfaces.Invitees;
using Tracker.Application.Interfaces.Requirements;
using Tracker.Application.Interfaces.Users;
using Tracker.Infrastructure.Persistence.Interceptors;
using Tracker.Infrastructure.Services.Events;
using Tracker.Infrastructure.Services.Groups;
using Tracker.Infrastructure.Services.Invitees;
using Tracker.Infrastructure.Services.Requirements;
using Tracker.Infrastructure.Services.Users;

namespace Tracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IGroupsService, GroupsService>();
        services.AddScoped<IEventsService, EventsService>();
        services.AddScoped<IInviteesService, InviteesService>();
        services.AddScoped<IRequirementsService, RequirementsService>();
        services.AddScoped<IRequirementsSynchronization, RequirementsSynchronization>();

        services.AddSingleton<AuditInterceptor>();
        
        return services;
    }
}