// Türkçe: HealthCheck servislerini eklemek için extension metodu
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace WebApi.Infrastructure.Observability;

public static class ObservabilityHealthCheckExtensions
{
    public static IServiceCollection AddObservabilityHealthChecks(this IServiceCollection services, IConfiguration config)
    {
        var connStr = config.GetConnectionString("DefaultConnection") ?? "";

        services.AddHealthChecks()
            .AddNpgSql(connStr, name: "postgres", tags: new[] { "db", "ready" });

        return services;
    }
}
