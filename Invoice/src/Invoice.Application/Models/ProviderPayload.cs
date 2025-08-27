namespace Invoice.Application.Models;

/// <summary>
/// Entegratöre gönderilecek ham veri formatı
/// </summary>
public class ProviderPayload
{
    /// <summary>
    /// Entegratör anahtarı (foriba/logo/mikro/...)
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;
    
    /// <summary>
    /// İdempotency anahtarı
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Fatura zarfı
    /// </summary>
    public InvoiceEnvelope Envelope { get; set; } = new();
    
    /// <summary>
    /// İmzalama modu (ProviderSign/SelfSign)
    /// </summary>
    public SignMode SignMode { get; set; }
    
    /// <summary>
    /// Ham veri (JSON/XML formatında)
    /// </summary>
    public string RawData { get; set; } = string.Empty;
    
    /// <summary>
    /// İmza (SelfSign modunda)
    /// </summary>
    public string? Signature { get; set; }
    
    /// <summary>
    /// Zaman damgası
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Ek parametreler (entegratör-özel)
    /// </summary>
    public Dictionary<string, object>? ExtraParameters { get; set; }
}

/// <summary>
/// İmzalama modu
/// </summary>
public enum SignMode
{
    /// <summary>
    /// Entegratör imzalar (JSON format)
    /// </summary>
    ProviderSign,
    
    /// <summary>
    /// Kendi imzamız (UBL + XAdES-BES)
    /// </summary>
    SelfSign
}
