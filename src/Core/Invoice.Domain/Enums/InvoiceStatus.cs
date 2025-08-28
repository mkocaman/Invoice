namespace Invoice.Domain.Enums;

/// <summary>
/// Fatura durumları
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// Oluşturuldu
    /// </summary>
    CREATED = 1,
    
    /// <summary>
    /// Gönderim için hazır
    /// </summary>
    READY_TO_SEND = 2,
    
    /// <summary>
    /// Gönderiliyor
    /// </summary>
    SENDING = 3,
    
    /// <summary>
    /// Gönderildi
    /// </summary>
    SENT = 4,
    
    /// <summary>
    /// Kabul edildi
    /// </summary>
    ACCEPTED = 5,
    
    /// <summary>
    /// Reddedildi
    /// </summary>
    REJECTED = 6,
    
    /// <summary>
    /// Hata oluştu
    /// </summary>
    ERROR = 7,
    
    /// <summary>
    /// İptal edildi
    /// </summary>
    CANCELLED = 8
}
