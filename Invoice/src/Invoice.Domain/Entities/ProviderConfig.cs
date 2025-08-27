using Invoice.Domain.Enums;

namespace Invoice.Domain.Entities;

/// <summary>
/// Entegratör konfigürasyonu - tenant bazlı ayarlar
/// </summary>
public class ProviderConfig : BaseEntity
{
    /// <summary>
    /// Entegratör anahtarı (foriba/logo/mikro/...)
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;
    
    /// <summary>
    /// API base URL
    /// </summary>
    public string ApiBaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// API anahtarı
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;
    
    /// <summary>
    /// API gizli anahtarı
    /// </summary>
    public string ApiSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// Webhook gizli anahtarı
    /// </summary>
    public string WebhookSecret { get; set; } = string.Empty;
    
    /// <summary>
    /// VKN/TCKN
    /// </summary>
    public string VknTckn { get; set; } = string.Empty;
    
    /// <summary>
    /// Firma adı
    /// </summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Şube kodu (opsiyonel)
    /// </summary>
    public string? BranchCode { get; set; }
    
    /// <summary>
    /// İmzalama modu
    /// </summary>
    public SignMode SignMode { get; set; } = SignMode.ProviderSign;
    
    /// <summary>
    /// Timeout süresi (saniye)
    /// </summary>
    public int TimeoutSec { get; set; } = 30;
    
    /// <summary>
    /// Retry sayısı override (opsiyonel)
    /// </summary>
    public int? RetryCountOverride { get; set; }
    
    /// <summary>
    /// Circuit breaker trip eşiği (opsiyonel)
    /// </summary>
    public int? CircuitTripThreshold { get; set; }
    

}
