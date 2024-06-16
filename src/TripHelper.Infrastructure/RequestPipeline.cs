using Microsoft.AspNetCore.Builder;
using TripHelper.Infrastructure.Common.Middleware;

namespace TripHelper.Infrastructure;

public static class RequestPipeline
{
    public static IApplicationBuilder AddInfrastructureMiddleware(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<EventualConsistencyMiddleware>();

        return builder;
    }
}