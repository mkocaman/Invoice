// Türkçe: Sağlayıcı sağlık durumlarını önbelleğe alır ve expose eder.
using Invoice.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Providers.Health
{
    public interface IProviderHealthService
    {
        Task<bool> IsHealthyAsync(IInvoiceProvider provider, CancellationToken ct);
        Task<Dictionary<string, bool>> SnapshotAsync(string? countryCode = null, CancellationToken ct = default);
    }

    public sealed class ProviderHealthService : IProviderHealthService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProviderHealthService> _logger;

        public ProviderHealthService(IMemoryCache cache, ILogger<ProviderHealthService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<bool> IsHealthyAsync(IInvoiceProvider provider, CancellationToken ct)
        {
            // Türkçe: 30 sn önbellek — sağlayıcıların health ping'lerini agresif çağırmamak için.
            var key = $"prov_health::{provider.ProviderType}";
            if (_cache.TryGetValue<bool>(key, out var cached)) return cached;

            bool ok = true; // Türkçe: Basit implementasyon - her zaman sağlıklı varsay
            try 
            { 
                // Türkçe: Mevcut interface'de HealthAsync yok, basit bir kontrol yap
                await Task.Delay(1, ct); // Simulated health check
            }
            catch (Exception ex) 
            { 
                _logger.LogWarning(ex, "Sağlayıcı health hatası: {ProviderType}", provider.ProviderType);
                ok = false;
            }

            _cache.Set(key, ok, TimeSpan.FromSeconds(30));
            return ok;
        }

        public async Task<Dictionary<string, bool>> SnapshotAsync(string? countryCode = null, CancellationToken ct = default)
        {
            var snapshot = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            // Not: Burada gerçek provider listesine erişmek için DI'dan registry çözmek daha doğru olurdu;
            // Marionette açısından basit tutuyoruz — API katmanı snapshot'ı registry üzerinden kuracak.
            await Task.CompletedTask;
            return snapshot;
        }
    }
}
