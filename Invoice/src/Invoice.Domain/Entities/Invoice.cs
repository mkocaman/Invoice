using System.ComponentModel.DataAnnotations;
using Invoice.Domain.Enums;
using Invoice.Domain.ValueObjects;

namespace Invoice.Domain.Entities;

/// <summary>
/// Fatura entity'si
/// </summary>
public class Invoice : BaseEntity
{
    /// <summary>
    /// Fatura numarası
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Fatura tipi
    /// </summary>
    public InvoiceType Type { get; set; }
    
    /// <summary>
    /// Fatura durumu
    /// </summary>
    public InvoiceStatus Status { get; set; }
    
    /// <summary>
    /// Fatura tarihi
    /// </summary>
    public DateTime InvoiceDate { get; set; }
    
    /// <summary>
    /// Fatura düzenleme tarihi
    /// </summary>
    public DateTime IssueDate { get; set; }
    
    /// <summary>
    /// Fatura toplam tutarı (TRY)
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// Fatura KDV tutarı (TRY)
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Fatura net tutarı (TRY)
    /// </summary>
    public decimal NetAmount { get; set; }
    
    /// <summary>
    /// Fatura para birimi (sabit TRY)
    /// </summary>
    public string Currency { get; set; } = "TRY";
    
    /// <summary>
    /// Fatura açıklaması
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Fatura notları
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Fatura PDF içeriği (base64)
    /// </summary>
    public string? PdfContent { get; set; }
    
    /// <summary>
    /// Fatura UUID (GİB)
    /// </summary>
    [MaxLength(100)]
    public string? Uuid { get; set; }
    
    /// <summary>
    /// Fatura UBL içeriği (base64)
    /// </summary>
    public string? UblContent { get; set; }
    
    /// <summary>
    /// Fatura JSON içeriği (provider-sign için)
    /// </summary>
    public string? JsonContent { get; set; }
    
    /// <summary>
    /// İmzalama modu
    /// </summary>
    public SigningMode SigningMode { get; set; }
    
    /// <summary>
    /// Sağlayıcı tipi
    /// </summary>
    public ProviderType ProviderType { get; set; }
    
    /// <summary>
    /// Sağlayıcı referans numarası
    /// </summary>
    [MaxLength(100)]
    public string? ProviderReferenceNumber { get; set; }
    
    /// <summary>
    /// Sağlayıcı yanıt mesajı
    /// </summary>
    [MaxLength(1000)]
    public string? ProviderResponseMessage { get; set; }
    
    /// <summary>
    /// Sağlayıcı hata kodu
    /// </summary>
    [MaxLength(50)]
    public string? ProviderErrorCode { get; set; }
    
    /// <summary>
    /// Sağlayıcı hata mesajı
    /// </summary>
    [MaxLength(1000)]
    public string? ProviderErrorMessage { get; set; }
    
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
    /// Gönderim tarihi
    /// </summary>
    public DateTime? SentAt { get; set; }
    
    /// <summary>
    /// Şarj seansı ID (şarj faturaları için)
    /// </summary>
    public Guid? ChargeSessionId { get; set; }
    
    /// <summary>
    /// Şarj seansı navigation property
    /// </summary>
    public virtual ChargeSession? ChargeSession { get; set; }
    
    /// <summary>
    /// Rapor ID (toplu faturalar için)
    /// </summary>
    public Guid? ReportId { get; set; }
    
    /// <summary>
    /// Rapor navigation property
    /// </summary>
    public virtual ReportId? Report { get; set; }
    
    /// <summary>
    /// Fatura detayları
    /// </summary>
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
