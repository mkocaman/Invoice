// Türkçe: Sağlayıcı kayıt/çözümleme servisi. Ülke + capability + kurallara göre sağlayıcı döndürür.
using Invoice.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Infrastructure.Providers;

namespace Infrastructure.Providers
{
    public interface IProviderRegistry
    {
        IReadOnlyList<IInvoiceProvider> GetAll();
        IReadOnlyList<IInvoiceProvider> GetByCountry(string countryCode);
        IInvoiceProvider? ResolveBest(string countryCode, string capability, string? preferKey = null);
    }

    public sealed class ProviderRegistry : IProviderRegistry
    {
        private readonly IEnumerable<IInvoiceProvider> _providers;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProviderRegistry> _logger;
        private readonly MultiProviderOptions _opts;

        public ProviderRegistry(IEnumerable<IInvoiceProvider> providers, IMemoryCache cache, IOptions<MultiProviderOptions> opts, ILogger<ProviderRegistry> logger)
        {
            _providers = providers;
            _cache = cache;
            _logger = logger;
            _opts = opts.Value;
        }

        public IReadOnlyList<IInvoiceProvider> GetAll() => _providers.ToList();

        public IReadOnlyList<IInvoiceProvider> GetByCountry(string countryCode)
            => _providers.Where(p => p.SupportsCountry(countryCode)).ToList();

        public IInvoiceProvider? ResolveBest(string countryCode, string capability, string? preferKey = null)
        {
            // Türkçe: Basit implementasyon - mevcut interface ile uyumlu
            var list = GetByCountry(countryCode).ToList();
            if (list.Count == 0) return null;

            // Türkçe: İlk uygun sağlayıcıyı döndür (şimdilik basit)
            return list.FirstOrDefault();
        }
    }
}
