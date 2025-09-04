using Microsoft.Extensions.DependencyInjection;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.TR.Providers;

namespace Invoice.Infrastructure.TR;

/// <summary>
/// TR Infrastructure katmanı için dependency injection konfigürasyonu
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// TR sağlayıcılarının hepsini DI'a ekle
    /// </summary>
    public static IServiceCollection AddInvoiceProvidersTR(this IServiceCollection services)
    {
        // Türkçe: TR sağlayıcılarının hepsini DI'a ekle
        services.AddScoped<IInvoiceProvider, ForibaProvider>();
        services.AddScoped<IInvoiceProvider, UyumsoftProvider>();
        services.AddScoped<IInvoiceProvider, KolayBiProvider>();
        services.AddScoped<IInvoiceProvider, ParasutProvider>();
        services.AddScoped<IInvoiceProvider, LogoProvider>();
        services.AddScoped<IInvoiceProvider, NetsisProvider>();
        services.AddScoped<IInvoiceProvider, MikroProvider>();
        services.AddScoped<IInvoiceProvider, DiaProvider>();
        services.AddScoped<IInvoiceProvider, IdeaProvider>();
        services.AddScoped<IInvoiceProvider, BizimHesapProvider>();

        return services;
    }
}
