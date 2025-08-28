using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.KZ.Providers;

namespace Invoice.Infrastructure.KZ;

/// <summary>
/// KZ Infrastructure katmanı için dependency injection konfigürasyonu
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// KZ sağlayıcılarını DI'a ekle
    /// </summary>
    public static IServiceCollection AddInvoiceProvidersKZ(this IServiceCollection services, IConfiguration configuration)
    {
        // Türkçe: Named HttpClient'ı yapılandır
        services.AddHttpClient("isEsfKz", (sp, client) => 
        {
            var baseUrl = configuration["Providers:IsEsf:BaseUrl"] ?? "https://esf.kgd.gov.kz";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Türkçe: KZ sağlayıcısını DI'a ekle
        services.AddScoped<IInvoiceProvider, IsEsfProvider>();

        return services;
    }
}
