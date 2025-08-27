using Microsoft.Extensions.DependencyInjection;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.Services;
using Invoice.Infrastructure.Providers;

namespace Invoice.Infrastructure;

/// <summary>
/// Infrastructure katmanı için dependency injection konfigürasyonu
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure servislerini DI container'a ekler
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // UBL XML servisleri
        services.AddScoped<IInvoiceUblService, InvoiceUblService>();
        services.AddScoped<IUblValidationService, UblValidationService>();
        
        // İmzalama servisleri
        services.AddScoped<ISigningService, MockSigningService>(); // Dev ortamında mock kullan
        
        // Provider konfigürasyonu servisleri
        services.AddScoped<IProviderConfigurationService, ProviderConfigurationService>();
        
        // UBL şema validasyonu için XML şema resolver
        services.AddSingleton<IUblSchemaResolver, UblSchemaResolver>();
        
        // Provider Factory
        services.AddScoped<IInvoiceProviderFactory, InvoiceProviderFactory>();
        
        // Provider Adapter'ları - Transient olarak kaydet
        services.AddTransient<IInvoiceProvider, ForibaProvider>();
        services.AddTransient<IInvoiceProvider, LogoProvider>();
        services.AddTransient<IInvoiceProvider, MikroProvider>();
        services.AddTransient<IInvoiceProvider, UyumsoftProvider>();
        services.AddTransient<IInvoiceProvider, KolayBiProvider>();
        services.AddTransient<IInvoiceProvider, ParasutProvider>();
        services.AddTransient<IInvoiceProvider, DiaProvider>();
        services.AddTransient<IInvoiceProvider, IdeaProvider>();
        services.AddTransient<IInvoiceProvider, BizimHesapProvider>();
        services.AddTransient<IInvoiceProvider, NetsisProvider>();
        
        return services;
    }
}
