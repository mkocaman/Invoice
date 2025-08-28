namespace Invoice.Application.Interfaces;

/// <summary>
/// Provider Factory - DI ile kayıtlı adapter'lardan providerKey eşleşeni döndürür
/// </summary>
public interface IInvoiceProviderFactory
{
    /// <summary>
    /// ProviderKey ve CountryCode'e göre uygun adapter'ı döndürür
    /// </summary>
    /// <param name="providerKey">Entegratör anahtarı (foriba/logo/mikro/...)</param>
    /// <param name="countryCode">Ülke kodu (TR/UZ/KZ)</param>
    /// <returns>Uygun provider adapter'ı</returns>
    /// <exception cref="InvalidOperationException">ProviderKey bulunamadığında</exception>
    IInvoiceProvider Resolve(string providerKey, string countryCode);
    
    /// <summary>
    /// ProviderKey'e göre uygun adapter'ı döndürür (geriye uyumluluk için)
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
    
    /// <summary>
    /// Belirtilen ülke için desteklenen provider'ları listeler
    /// </summary>
    /// <param name="countryCode">Ülke kodu (TR/UZ/KZ)</param>
    /// <returns>Provider adları listesi</returns>
    IEnumerable<string> GetSupportedProviders(string countryCode);
}
