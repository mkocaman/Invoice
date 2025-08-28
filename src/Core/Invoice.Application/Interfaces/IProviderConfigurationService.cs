using Invoice.Domain.Entities;

namespace Invoice.Application.Interfaces;

/// <summary>
/// Provider konfigürasyon servisi sözleşmesi
/// </summary>
public interface IProviderConfigurationService
{
    /// <summary>
    /// Tenant ve Provider key'e göre konfigürasyon getirir
    /// </summary>
    Task<ProviderConfig?> GetAsync(string tenantId, string providerKey);
    
    /// <summary>
    /// Konfigürasyonu oluşturur
    /// </summary>
    Task<ProviderConfig> CreateAsync(ProviderConfig configuration);
    
    /// <summary>
    /// Konfigürasyonu günceller
    /// </summary>
    Task<ProviderConfig> UpdateAsync(ProviderConfig configuration);
    
    /// <summary>
    /// Konfigürasyonu siler
    /// </summary>
    Task DeleteAsync(string tenantId, string providerKey);
    
    /// <summary>
    /// Tenant'a ait tüm konfigürasyonları listeler
    /// </summary>
    Task<IEnumerable<ProviderConfig>> ListAsync(string tenantId);
}
