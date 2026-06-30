using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Services.Groups;
using Tracker.Application.Services.Users;
using Tracker.Infrastructure.Services.Groups;
using Tracker.Infrastructure.Services.Users;

namespace Tracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUsersService, UsersService>();
        services.AddScoped<IGroupsService, GroupsService>();
        
        return services;
    }
}