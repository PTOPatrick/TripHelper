using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TripHelper.Application.Common.Interfaces;
using TripHelper.Domain.Common.Interfaces;
using TripHelper.Infrastructure.Authentication.PasswordHasher;
using TripHelper.Infrastructure.Authentication.TokenGenerator;
using TripHelper.Infrastructure.Common.Persistence;
using TripHelper.Infrastructure.Members.Persistence;
using TripHelper.Infrastructure.Trip.Persistence;
using TripHelper.Infrastructure.Users.Persistence;

namespace TripHelper.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string dbConnectionString, JwtSettings jwtSettings)
    {
        return services
            .AddAuthentication(jwtSettings)
            .AddPersistence(dbConnectionString);
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

    public static IServiceCollection AddAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {

        services.AddSingleton(Options.Create(jwtSettings));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            });


        return services;
    }
}