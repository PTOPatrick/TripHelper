using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Infrastructure.Common.Persistence;
using TripHelper.Infrastructure.Members.Persistence;
using TripHelper.Infrastructure.Trip.Persistence;
using TripHelper.Infrastructure.Users.Persistence;

namespace TripHelper.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbConnectionString)
    {
        return services.AddPersistence(dbConnectionString);
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, string dbConnectionString)
    {
        services.AddDbContext<TripHelperDbContext>(options => options.UseSqlServer(dbConnectionString));
        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<IMembersRepository, MembersRepository>();
        services.AddScoped<ITripsRepository, TripsRepository>();
        services.AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<TripHelperDbContext>());
        
        return services;
    }
}