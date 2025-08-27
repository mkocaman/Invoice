

namespace Invoice.Domain.Entities;

/// <summary>
/// İdempotency anahtarı entity'si - aynı isteğin tekrar işlenmesini engeller
/// </summary>
public class IdempotencyKey : BaseEntity
{
    /// <summary>
    /// İdempotency anahtarı (unique)
    /// </summary>
    public string Key { get; set; } = string.Empty;
    
    /// <summary>
    /// İsteğin gövde hash'i (opsiyonel)
    /// </summary>
    public string? Hash { get; set; }
    
    /// <summary>
    /// HTTP metodu
    /// </summary>
    public string Method { get; set; } = string.Empty;
    
    /// <summary>
    /// İstek yolu
    /// </summary>
    public string Path { get; set; } = string.Empty;
    
    /// <summary>
    /// İlk görülme zamanı
    /// </summary>
    public DateTime FirstSeenAt { get; set; }
    
    /// <summary>
    /// Geçerlilik süresi
    /// </summary>
    public DateTime ExpiresAt { get; set; }
    
    /// <summary>
    /// Son HTTP durum kodu
    /// </summary>
    public int? LastStatusCode { get; set; }
}
