// Türkçe: Health check'leri (liveness/readiness) ekleyen eklenti.
// Liveness: sadece process ayakta mı? Readiness: DB/Rabbit/Provider erişimi çalışıyor mu?
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi.Infrastructure.Health
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddInvoiceHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("Default")
                       ?? "Host=localhost;Port=5433;Database=invoice_db;Username=postgres;Password=1453;";

            // Türkçe: RabbitMQ connection string (amqp) — yoksa dev için guest:guest varsayalım
            var amqp = configuration["RabbitMQ:ConnectionString"] ?? "amqp://guest:guest@localhost:5672/";

            var hc = services.AddHealthChecks();

            // Türkçe: Basit "self" liveness check (her zaman Healthy)
            hc.AddCheck("self", () => HealthCheckResult.Healthy("alive"), tags: new[] { "live" });

            // Türkçe: Readiness — PostgreSQL
            hc.AddNpgSql(conn, name: "postgresql", tags: new[] { "ready" });

            // Türkçe: Readiness — RabbitMQ (temporarily disabled due to RabbitMQ.Client 7.0 compatibility issues)
            // hc.AddRabbitMQ(rabbitConnectionString: amqp, name: "rabbitmq", tags: new[] { "ready" });

            // Not: İleride Provider API'leri için custom health check ekleyebilirsin (ör. ping endpointleri).

            return services;
        }
    }
}
