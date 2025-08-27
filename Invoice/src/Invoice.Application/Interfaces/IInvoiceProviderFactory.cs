namespace Invoice.Application.Interfaces;

/// <summary>
/// Provider Factory - DI ile kayıtlı adapter'lardan providerKey eşleşeni döndürür
/// </summary>
public interface IInvoiceProviderFactory
{
    /// <summary>
    /// ProviderKey'e göre uygun adapter'ı döndürür
    /// </summary>
    /// <param name="providerKey">Entegratör anahtarı (foriba/logo/mikro/...)</param>
    /// <returns>Uygun provider adapter'ı</returns>
    /// <exception cref="InvalidOperationException">ProviderKey bulunamadığında</exception>
    IInvoiceProvider Resolve(string providerKey);
    
    /// <summary>
    /// Desteklenen provider'ları listeler
    /// </summary>
    /// <returns>Provider adları listesi</returns>
    IEnumerable<string> GetSupportedProviders();
}
