using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.Data;
using Invoice.Infrastructure.Providers;
using Invoice.Infrastructure.Services;

namespace Invoice.Infrastructure;

/// <summary>
/// Infrastructure katmanı için dependency injection konfigürasyonu
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Infrastructure servislerini DI container'a ekler
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Entity Framework
        services.AddDbContext<InvoiceDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Provider Factory
        services.AddScoped<IInvoiceProviderFactory, InvoiceProviderFactory>();

        // Provider Adapters
        services.AddScoped<ForibaProvider>();
        services.AddScoped<LogoProvider>();
        services.AddScoped<MikroProvider>();
        services.AddScoped<UyumsoftProvider>();
        services.AddScoped<KolayBiProvider>();
        services.AddScoped<ParasutProvider>();
        services.AddScoped<DiaProvider>();
        services.AddScoped<IdeaProvider>();
        services.AddScoped<BizimHesapProvider>();
        services.AddScoped<NetsisProvider>();

        // Services
        services.AddScoped<IInvoiceUblService, InvoiceUblService>();
        services.AddScoped<IUblValidationService, MockUblValidationService>();
        services.AddScoped<IProviderConfigurationService, ProviderConfigurationService>();
        services.AddScoped<IWebhookSignatureValidator, MockWebhookSignatureValidator>();
        services.AddScoped<SeedService>();

        return services;
    }

    public static IServiceCollection AddInvoiceProviders(this IServiceCollection services)
    {
        // Türkçe: Named HttpClient'lar
        services.AddHttpClient("dia");
        services.AddHttpClient("mikro");
        services.AddHttpClient("bizimhesap");
        services.AddHttpClient("netsis");
        services.AddHttpClient("idea");

        // Türkçe: Sağlayıcı implementasyonları
        services.AddTransient<Providers.DiaProvider>();
        services.AddTransient<Providers.MikroProvider>();
        services.AddTransient<Providers.BizimHesapProvider>();
        services.AddTransient<Providers.NetsisProvider>();
        services.AddTransient<Providers.IdeaProvider>();

        return services;
    }
}
