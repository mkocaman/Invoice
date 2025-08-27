using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Retry job entity'si
/// </summary>
public class RetryJob : BaseEntity
{
    /// <summary>
    /// İş tipi
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string JobType { get; set; } = string.Empty;
    
    /// <summary>
    /// İş verisi (JSON)
    /// </summary>
    [Required]
    public string JobData { get; set; } = string.Empty;
    
    /// <summary>
    /// İş durumu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Deneme sayısı
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Maksimum deneme sayısı
    /// </summary>
    public int MaxRetryCount { get; set; }
    
    /// <summary>
    /// Son deneme tarihi
    /// </summary>
    public DateTime? LastRetryDate { get; set; }
    
    /// <summary>
    /// Sonraki deneme tarihi
    /// </summary>
    public DateTime? NextRetryDate { get; set; }
    
    /// <summary>
    /// Tamamlanma tarihi
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    
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
    /// İşlem süresi (ms)
    /// </summary>
    public long? DurationMs { get; set; }
    
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
    /// Öncelik (düşük sayı = yüksek öncelik)
    /// </summary>
    public int Priority { get; set; }
    
    /// <summary>
    /// İş etiketleri (JSON array)
    /// </summary>
    public string? Tags { get; set; }
}
