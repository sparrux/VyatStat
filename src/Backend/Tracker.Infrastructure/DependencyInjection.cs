using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Services.Events;
using Tracker.Application.Services.Groups;
using Tracker.Application.Services.Invitees;
using Tracker.Application.Services.Requirements;
using Tracker.Application.Services.Users;
using Tracker.Infrastructure.Services.GroupEvents;
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
        services.AddScoped<IGroupEventsService, GroupEventsService>();
        services.AddScoped<IInviteesService, InviteesService>();
        
        services.AddScoped<IRequirementsSynchronization, RequirementsSynchronization>();
        
        return services;
    }
}