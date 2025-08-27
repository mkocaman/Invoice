using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Provider anahtarına göre uygun adapter örneğini döndürür
/// </summary>
public interface IInvoiceProviderFactory
{
    /// <summary>
    /// Provider anahtarına göre adapter çözümler
    /// </summary>
    /// <param name="providerKey">Entegratör anahtarı (foriba/logo/mikro/...)</param>
    /// <returns>Uygun adapter örneği</returns>
    /// <exception cref="ArgumentException">Bilinmeyen provider anahtarı</exception>
    IInvoiceProvider Resolve(string providerKey);
    
    /// <summary>
    /// Desteklenen provider anahtarlarını listeler
    /// </summary>
    /// <returns>Desteklenen provider anahtarları</returns>
    IEnumerable<string> GetSupportedProviders();
}
