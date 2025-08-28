// Türkçe: IInvoiceProvider implementasyonlarını reflection ile otomatik kaydeder.
// Lifetime düzeltmeleri:
// - IInvoiceUblService => Scoped
// - IInvoiceProviderFactory => Scoped
// - IInvoiceProvider => Transient (scoped service tüketebilir, request scope içinde resolve edilecek)

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.Services;

namespace Invoice.Infrastructure.Providers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInvoiceProviders(this IServiceCollection services)
        {
            // Genel HttpClient
            services.AddHttpClient();

            // 🔸 UBL servis kaydı (Scoped)
            // Not: Projedeki somut sınıf adınız farklıysa onu yazın.
            services.AddScoped<IInvoiceUblService, InvoiceUblService>();

            // 🔸 Factory kaydı (Scoped) — Singleton OLMAZ (Scoped bağımlılık var)
            services.TryAddScoped<IInvoiceProviderFactory, InvoiceProviderFactory>();

            // 🔸 Reflection ile tüm IInvoiceProvider implementasyonlarını kaydet
            var infraAsm = typeof(DependencyInjection).Assembly;
            var appAsm = typeof(Invoice.Application.Interfaces.IInvoiceProvider).Assembly;

            var candidates = infraAsm
                .GetTypes()
                .Concat(appAsm.GetTypes())
                .Where(t =>
                    t is { IsClass: true, IsAbstract: false, IsPublic: true } &&
                    typeof(IInvoiceProvider).IsAssignableFrom(t))
                .Distinct()
                .ToList();

            foreach (var t in candidates)
            {
                // Transient yeterli; request scope içinde resolve edilecek.
                services.AddTransient(typeof(IInvoiceProvider), t);
            }

            return services;
        }
    }
}
