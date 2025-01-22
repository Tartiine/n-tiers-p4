using Microsoft.Extensions.DependencyInjection;
using NTiersP4.Domain.Services;

namespace NTiersP4.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<GameService>();
        services.AddScoped<PlayerService>();

        return services;
    }
}