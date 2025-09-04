// Türkçe Açıklama:
// Tüm komutlar için ortak konfigürasyon yükleyici.
// Önce appsettings.json, sonra (varsa) appsettings.{ENV}.json, en sonda Env Vars.
// ENV adı CLI'dan (--env) gelebilir; yoksa DOTNET_ENVIRONMENT / ASPNETCORE_ENVIRONMENT
// veya 'Development' varsayılır (Console app'te yerel test kolaylığı için).

using Microsoft.Extensions.Configuration;

namespace Invoice.SandboxRunner;

public static class ConfigLoader
{
    public static IConfiguration Load(string? cliEnv = null)
    {
        // Türkçe: Base path olarak bin/ (AppContext.BaseDirectory) kullanıyoruz.
        var basePath = AppContext.BaseDirectory;

        // Türkçe: ENV belirleme önceliği: CLI > DOTNET_ENVIRONMENT > ASPNETCORE_ENVIRONMENT > "Development"
        var env = !string.IsNullOrWhiteSpace(cliEnv)
            ? cliEnv
            : (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
               ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
               ?? "Development");

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            // ÖNCE ana dosya
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            // SONRA env dosyası (üstüne binsin)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            // En sonda env değişkenleri (en yüksek öncelik)
            .AddEnvironmentVariables();

        var config = builder.Build();
        return config;
    }
}
