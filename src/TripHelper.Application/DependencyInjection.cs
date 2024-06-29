using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TripHelper.Application.Common.Behaviors;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Application.Common.Services.Authorization;

namespace TripHelper.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
        });

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        services.AddScoped<IAuthorizationService, AuthorizationService>();

        return services;
    }
}