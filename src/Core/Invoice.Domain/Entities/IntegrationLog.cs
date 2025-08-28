using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Entegrasyon log entity'si
/// </summary>
public class IntegrationLog : BaseEntity
{
    /// <summary>
    /// Log seviyesi
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Level { get; set; } = string.Empty;
    
    /// <summary>
    /// Log mesajı
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Log detayları (JSON)
    /// </summary>
    public string? Details { get; set; }
    
    /// <summary>
    /// İşlem tipi
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string OperationType { get; set; } = string.Empty;
    
    /// <summary>
    /// İşlem ID
    /// </summary>
    public Guid? OperationId { get; set; }
    
    /// <summary>
    /// Sağlayıcı tipi
    /// </summary>
    [MaxLength(50)]
    public string? ProviderType { get; set; }
    
    /// <summary>
    /// HTTP durum kodu
    /// </summary>
    public int? HttpStatusCode { get; set; }
    
    /// <summary>
    /// HTTP yanıt içeriği
    /// </summary>
    public string? HttpResponseContent { get; set; }
    
    /// <summary>
    /// HTTP istek içeriği
    /// </summary>
    public string? HttpRequestContent { get; set; }
    
    /// <summary>
    /// HTTP istek URL'i
    /// </summary>
    [MaxLength(500)]
    public string? HttpRequestUrl { get; set; }
    
    /// <summary>
    /// HTTP istek metodu
    /// </summary>
    [MaxLength(10)]
    public string? HttpRequestMethod { get; set; }
    
    /// <summary>
    /// İşlem süresi (ms)
    /// </summary>
    public long? DurationMs { get; set; }
    
    /// <summary>
    /// Hata detayları
    /// </summary>
    public string? ExceptionDetails { get; set; }
    
    /// <summary>
    /// Correlation ID
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
    
    /// <summary>
    /// User ID
    /// </summary>
    [MaxLength(100)]
    public string? UserId { get; set; }
    
    /// <summary>
    /// IP adresi
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    /// <summary>
    /// User Agent
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }
}
