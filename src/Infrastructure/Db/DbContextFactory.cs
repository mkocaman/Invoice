// Türkçe Açıklama:
// PostgreSQL için DbContext konfigürasyonu.
// EF Core loglarını Serilog'a yönlendirir.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Db;

public static class DbContextFactory
{
    public static void Configure(DbContextOptionsBuilder builder, ILoggerFactory loggerFactory, IConfiguration cfg, bool isDevelopment)
    {
        var cs = cfg.GetConnectionString("Default");
        builder
            .UseNpgsql(cs, o => o.EnableRetryOnFailure(5)) // Türkçe: Transient hata retry
            .UseLoggerFactory(loggerFactory)               // Türkçe: EF loglarını Serilog'a yönlendir
            .EnableSensitiveDataLogging(isDevelopment)     // Türkçe: Parametreleri sadece dev ortamda logla
            .ConfigureWarnings(w =>
            {
                w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
            })
            .LogTo(
                msg => loggerFactory.CreateLogger("EFCore.SQL").LogInformation(msg),
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information
            );
    }
}
