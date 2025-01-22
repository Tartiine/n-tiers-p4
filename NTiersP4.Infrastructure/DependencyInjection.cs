using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NTiersP4.Domain.Repositories;
using NTiersP4.Infrastructure.Repositories;

namespace NTiersP4.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)  
    {
        services.AddDbContext<DatabaseContext>(options =>
            options
                .UseSqlite($"Data Source={connectionString}")
                .LogTo(Console.WriteLine, LogLevel.Information));

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IGridRepository, GridRepository>();

        return services;
    }
}