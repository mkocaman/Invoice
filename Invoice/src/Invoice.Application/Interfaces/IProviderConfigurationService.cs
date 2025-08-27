namespace Invoice.Application.Interfaces;

/// <summary>
/// Provider konfigürasyon modeli
/// </summary>
public class ProviderConfiguration
{
    public int Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string ProviderKey { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string VknTckn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Provider konfigürasyon servisi sözleşmesi
/// </summary>
public interface IProviderConfigurationService
{
    /// <summary>
    /// Provider ID'sine göre konfigürasyon getirir
    /// </summary>
    Task<ProviderConfiguration?> GetProviderConfigurationAsync(int providerId);
    
    /// <summary>
    /// Konfigürasyonu kaydeder
    /// </summary>
    Task<ProviderConfiguration> SaveProviderConfigurationAsync(ProviderConfiguration configuration);
    
    /// <summary>
    /// Aktif konfigürasyonları listeler
    /// </summary>
    Task<IEnumerable<ProviderConfiguration>> GetActiveProviderConfigurationsAsync();
    
    /// <summary>
    /// Konfigürasyonu deaktif eder
    /// </summary>
    Task DeactivateProviderConfigurationAsync(int providerId);
    
    /// <summary>
    /// Konfigürasyonu siler
    /// </summary>
    Task DeleteProviderConfigurationAsync(int providerId);
    
    /// <summary>
    /// Konfigürasyonu test eder
    /// </summary>
    Task<bool> TestProviderConfigurationAsync(int providerId);
}
