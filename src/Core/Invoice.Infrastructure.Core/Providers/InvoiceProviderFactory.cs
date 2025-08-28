// Türkçe: Factory, constructor'da IEnumerable<IInvoiceProvider> alır (scoped).
// Root provider üzerinden GetServices/ServiceProvider kullanılmaz.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Providers
{
    public class InvoiceProviderFactory : IInvoiceProviderFactory
    {
        private readonly IReadOnlyList<IInvoiceProvider> _providers;
        private readonly ILogger<InvoiceProviderFactory> _logger;

        public InvoiceProviderFactory(IEnumerable<IInvoiceProvider> providers,
                                      ILogger<InvoiceProviderFactory> logger)
        {
            // Türkçe: Scoped kapsamda gelen provider listesini naklediyoruz
            _providers = providers?.ToList() ?? new List<IInvoiceProvider>();
            _logger = logger;
        }

        // Türkçe yorum: countryCode (örn: "TR") ve providerKey (örn: "Foriba") ile sağlayıcıyı bul.
        public IInvoiceProvider Resolve(string countryCode, string providerKey)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("countryCode boş olamaz", nameof(countryCode));

            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentException("providerKey boş olamaz", nameof(providerKey));

            var match = _providers.FirstOrDefault(p =>
                p.SupportsCountry(countryCode) &&
                string.Equals(Enum.GetName(typeof(ProviderType), p.ProviderType), providerKey, StringComparison.OrdinalIgnoreCase));

            return match ?? throw new InvalidOperationException($"Sağlayıcı bulunamadı: country={countryCode}, provider={providerKey}");
        }

        public IInvoiceProvider Resolve(string providerKey)
        {
            if (string.IsNullOrWhiteSpace(providerKey))
                throw new ArgumentException("providerKey boş olamaz.", nameof(providerKey));

            // Türkçe: Enum.ToString() ile birebir eşleşme (case-insensitive)
            var match = _providers.FirstOrDefault(p =>
                string.Equals(p.ProviderType.ToString(), providerKey, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                var existing = string.Join(", ", _providers.Select(p => p.ProviderType.ToString()));
                _logger.LogError("ProviderKey bulunamadı. ProviderKey: {ProviderKey}, Mevcut: {Existing}", providerKey, existing);
                throw new InvalidOperationException($"'{providerKey}' için uygun provider bulunamadı. Mevcut: {existing}");
            }

            return match;
        }

        // Türkçe yorum: Ülkeye göre desteklenen sağlayıcı anahtarlarını döndür.
        public IEnumerable<string> GetSupportedProviders(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("countryCode boş olamaz", nameof(countryCode));

            return _providers
                .Where(p => p.SupportsCountry(countryCode))
                .Select(p => Enum.GetName(typeof(ProviderType), p.ProviderType)!)
                .OrderBy(x => x)
                .ToArray();
        }

        public IEnumerable<string> GetSupportedProviders()
            => _providers.Select(p => p.ProviderType.ToString()).Distinct().OrderBy(x => x);
    }
}
