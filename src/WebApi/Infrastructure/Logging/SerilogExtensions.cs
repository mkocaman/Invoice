// Türkçe: Serilog yapılandırmasını yapan eklenti. Dosya bazlı rolling loglar (app/db/rabbitmq) örneği içerir.
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace WebApi.Infrastructure.Logging
{
    public static class SerilogExtensions
    {
        // Türkçe: Host builder tarafında UseSerilog(delegate) ile de bağlanabilir; burada service eklentisi olarak bırakıyoruz.
        public static void ConfigureSerilogFromConfiguration(IConfiguration configuration)
        {
            // Türkçe: appsettings'lerden okuyarak minimal bir Serilog konfigürasyonu kur.
            var logDir = configuration["Logging:Paths:Root"] ?? "logs";
            var appPath = System.IO.Path.Combine(logDir, "app", "app-.log");
            var dbPath  = System.IO.Path.Combine(logDir, "db", "db-.log");
            var mqPath  = System.IO.Path.Combine(logDir, "rabbitmq", "rabbitmq-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .WriteTo.Console()
                // Türkçe: Uygulama logları (genel)
                .WriteTo.File(appPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, shared: true)
                // Türkçe: EF Core/RDBMS kategorileri için ayrı dosya (Filter ile ayrıştırılabilir)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le => le.Properties.ContainsKey("SourceContext") && le.Properties["SourceContext"].ToString().Contains("Microsoft.EntityFrameworkCore"))
                    .WriteTo.File(dbPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, shared: true))
                // Türkçe: RabbitMQ kategorileri için ayrı dosya
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(le => le.Properties.ContainsKey("SourceContext") && le.Properties["SourceContext"].ToString().Contains("Rabbit"))
                    .WriteTo.File(mqPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14, shared: true))
                .CreateLogger();
        }
    }
}
