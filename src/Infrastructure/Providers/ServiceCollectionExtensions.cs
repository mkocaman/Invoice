// Türkçe: Multi-provider servis kayıtları (DI)
using Infrastructure.Providers.Health;
using Infrastructure.Providers.Selection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Infrastructure.Providers;

namespace Infrastructure.Providers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultiProviderCore(this IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddScoped<IProviderRegistry, ProviderRegistry>();
            services.AddScoped<IProviderHealthService, ProviderHealthService>();
            services.AddScoped<IProviderInvoker, ProviderInvoker>();

            // Türkçe: MultiProviderOptions'ı DI'a bağla
            services.Configure<MultiProviderOptions>(options => { });

            return services;
        }
    }
}
