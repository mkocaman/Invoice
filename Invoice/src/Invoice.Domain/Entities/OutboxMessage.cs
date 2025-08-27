using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Outbox message entity'si (MassTransit için)
/// </summary>
public class OutboxMessage : BaseEntity
{
    /// <summary>
    /// Mesaj ID
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string MessageId { get; set; } = string.Empty;
    
    /// <summary>
    /// Mesaj tipi
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string MessageType { get; set; } = string.Empty;
    
    /// <summary>
    /// Mesaj içeriği (JSON)
    /// </summary>
    [Required]
    public string MessageContent { get; set; } = string.Empty;
    
    /// <summary>
    /// Kuyruk adı
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string QueueName { get; set; } = string.Empty;
    
    /// <summary>
    /// Mesaj durumu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
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
    /// Hata mesajı
    /// </summary>
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Correlation ID
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
    
    /// <summary>
    /// Conversation ID
    /// </summary>
    [MaxLength(100)]
    public string? ConversationId { get; set; }
}
