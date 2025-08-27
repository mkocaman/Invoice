using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Webhook log entity'si
/// </summary>
public class WebhookLog : BaseEntity
{
    /// <summary>
    /// Webhook URL'i
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string WebhookUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Webhook içeriği (JSON)
    /// </summary>
    [Required]
    public string WebhookContent { get; set; } = string.Empty;
    
    /// <summary>
    /// Webhook durumu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// HTTP durum kodu
    /// </summary>
    public int? HttpStatusCode { get; set; }
    
    /// <summary>
    /// HTTP yanıt içeriği
    /// </summary>
    public string? HttpResponseContent { get; set; }
    
    /// <summary>
    /// HTTP yanıt başlıkları (JSON)
    /// </summary>
    public string? HttpResponseHeaders { get; set; }
    
    /// <summary>
    /// Gönderim deneme sayısı
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Son gönderim deneme tarihi
    /// </summary>
    public DateTime? LastRetryDate { get; set; }
    
    /// <summary>
    /// Sonraki gönderim deneme tarihi
    /// </summary>
    public DateTime? NextRetryDate { get; set; }
    
    /// <summary>
    /// Gönderilme tarihi
    /// </summary>
    public DateTime? SentDate { get; set; }
    
    /// <summary>
    /// İşlem süresi (ms)
    /// </summary>
    public long? DurationMs { get; set; }
    
    /// <summary>
    /// Hata mesajı
    /// </summary>
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    
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
    /// İşlem ID
    /// </summary>
    public Guid? OperationId { get; set; }
    
    /// <summary>
    /// İşlem tipi
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string OperationType { get; set; } = string.Empty;
}
