

namespace Invoice.Domain.Entities;

/// <summary>
/// İdempotency anahtarı entity'si - aynı isteğin tekrar işlenmesini engeller
/// </summary>
public class IdempotencyKey : BaseEntity
{
    /// <summary>
    /// Tenant ID (müşteri organizasyonu)
    /// </summary>
    public new string TenantId { get; set; } = string.Empty;
    
    /// <summary>
    /// İdempotency anahtarı (unique)
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// İsteğin gövde hash'i
    /// </summary>
    public string RequestHash { get; set; } = string.Empty;
    
    /// <summary>
    /// HTTP metodu
    /// </summary>
    public string Method { get; set; } = string.Empty;
    
    /// <summary>
    /// İstek yolu
    /// </summary>
    public string Path { get; set; } = string.Empty;
    
    /// <summary>
    /// Oluşturulma zamanı
    /// </summary>
    public new DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Geçerlilik süresi
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Kullanılma zamanı (null ise henüz kullanılmamış)
    /// </summary>
    public DateTime? UsedAt { get; set; }
    
    /// <summary>
    /// Durum (PENDING, COMPLETED, FAILED)
    /// </summary>
    public string Status { get; set; } = "PENDING";
    
    /// <summary>
    /// İlk görülme zamanı
    /// </summary>
    public DateTime FirstSeenAt { get; set; }
    
    /// <summary>
    /// Son HTTP status kodu
    /// </summary>
    public int? LastStatusCode { get; set; }
    
    /// <summary>
    /// İsteğin hash'i (gövde + header'lar)
    /// </summary>
    public string Hash { get; set; } = string.Empty;
}
