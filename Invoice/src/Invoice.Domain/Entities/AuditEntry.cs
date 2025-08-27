using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Audit entry entity'si
/// </summary>
public class AuditEntry : BaseEntity
{
    /// <summary>
    /// Tablo adı
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;
    
    /// <summary>
    /// Kayıt ID
    /// </summary>
    public Guid RecordId { get; set; }
    
    /// <summary>
    /// İşlem tipi (INSERT, UPDATE, DELETE)
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty;
    
    /// <summary>
    /// Eski değerler (JSON)
    /// </summary>
    public string? OldValues { get; set; }
    
    /// <summary>
    /// Yeni değerler (JSON)
    /// </summary>
    public string? NewValues { get; set; }
    
    /// <summary>
    /// Değişen alanlar (JSON array)
    /// </summary>
    public string? ChangedColumns { get; set; }
    
    /// <summary>
    /// User ID
    /// </summary>
    [MaxLength(100)]
    public string? UserId { get; set; }
    
    /// <summary>
    /// User adı
    /// </summary>
    [MaxLength(200)]
    public string? UserName { get; set; }
    
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
    
    /// <summary>
    /// İşlem açıklaması
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Correlation ID
    /// </summary>
    [MaxLength(100)]
    public string? CorrelationId { get; set; }
}
