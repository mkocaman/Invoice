using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Tüm entity'ler için temel sınıf
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Benzersiz kimlik
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Oluşturulma tarihi
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Güncellenme tarihi
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Kiracı kimliği (multi-tenant için)
    /// </summary>
    public string TenantId { get; set; } = string.Empty;
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool IsActive { get; set; } = true;
}
