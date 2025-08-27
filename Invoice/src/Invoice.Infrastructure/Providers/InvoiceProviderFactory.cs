using Microsoft.Extensions.DependencyInjection;
using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Provider Factory implementasyonu - DI ile kayıtlı adapter'lardan providerKey eşleşeni döndürür
/// </summary>
public class InvoiceProviderFactory : IInvoiceProviderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InvoiceProviderFactory> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public InvoiceProviderFactory(IServiceProvider serviceProvider, ILogger<InvoiceProviderFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// ProviderKey'e göre uygun adapter'ı döndürür
    /// </summary>
    public IInvoiceProvider Resolve(string providerKey)
    {
        _logger.LogDebug("Provider adapter'ı aranıyor. ProviderKey: {ProviderKey}", providerKey);

        if (string.IsNullOrWhiteSpace(providerKey))
        {
            throw new ArgumentException("ProviderKey boş olamaz", nameof(providerKey));
        }

        // Tüm IInvoiceProvider implementasyonlarını al
        var providers = _serviceProvider.GetServices<IInvoiceProvider>();
        
        // ProviderKey'e göre eşleşen adapter'ı bul
        var provider = providers.FirstOrDefault(p => 
            string.Equals(p.Name, providerKey, StringComparison.OrdinalIgnoreCase));

        if (provider == null)
        {
            var availableProviders = providers.Select(p => p.Name).ToList();
            _logger.LogError("ProviderKey bulunamadı. ProviderKey: {ProviderKey}, Mevcut: {AvailableProviders}", 
                providerKey, string.Join(", ", availableProviders));
            
            throw new InvalidOperationException($"'{providerKey}' providerKey'i için uygun adapter bulunamadı. " +
                $"Mevcut provider'lar: {string.Join(", ", availableProviders)}");
        }

        _logger.LogDebug("Provider adapter'ı bulundu. ProviderKey: {ProviderKey}, Adapter: {AdapterType}", 
            providerKey, provider.GetType().Name);

        return provider;
    }
    
    /// <summary>
    /// Desteklenen provider'ları listeler
    /// </summary>
    public IEnumerable<string> GetSupportedProviders()
    {
        var providers = _serviceProvider.GetServices<IInvoiceProvider>();
        return providers.Select(p => p.Name).ToList();
    }
}
