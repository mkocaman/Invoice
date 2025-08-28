using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.UZ.Providers;

namespace Invoice.Infrastructure.UZ;

/// <summary>
/// UZ Infrastructure katmanı için dependency injection konfigürasyonu
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// UZ sağlayıcılarını DI'a ekle
    /// </summary>
    public static IServiceCollection AddInvoiceProvidersUZ(this IServiceCollection services, IConfiguration configuration)
    {
        // Türkçe: Named HttpClient'ları yapılandır
        services.AddHttpClient("fakturaUz", (sp, client) => 
        {
            var baseUrl = configuration["Providers:FakturaUz:BaseUrl"] ?? "https://api.faktura.uz";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("didoxUz", (sp, client) => 
        {
            var baseUrl = configuration["Providers:DidoxUz:BaseUrl"] ?? "https://api.didox.uz";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Türkçe: UZ sağlayıcılarını DI'a ekle
        services.AddScoped<IInvoiceProvider, FakturaUzProvider>();
        services.AddScoped<IInvoiceProvider, DidoxProvider>();

        return services;
    }
}
