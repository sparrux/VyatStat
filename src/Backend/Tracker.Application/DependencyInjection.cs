using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Tracker.Application.Validators.Group;

namespace Tracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateGroupRequestValidator>();

        return services;
    }
}