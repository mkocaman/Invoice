using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Fatura detay entity'si
/// </summary>
public class InvoiceDetail : BaseEntity
{
    /// <summary>
    /// Fatura ID
    /// </summary>
    public Guid InvoiceId { get; set; }
    
    /// <summary>
    /// Fatura navigation property
    /// </summary>
    public virtual Invoice Invoice { get; set; } = null!;
    
    /// <summary>
    /// Satır numarası
    /// </summary>
    public int LineNumber { get; set; }
    
    /// <summary>
    /// Ürün/hizmet adı
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ItemName { get; set; } = string.Empty;
    
    /// <summary>
    /// Ürün/hizmet açıklaması
    /// </summary>
    [MaxLength(500)]
    public string? ItemDescription { get; set; }
    
    /// <summary>
    /// Miktar
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Birim
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Unit { get; set; } = string.Empty;
    
    /// <summary>
    /// Birim fiyat (TRY)
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Net tutar (TRY)
    /// </summary>
    public decimal NetAmount { get; set; }
    
    /// <summary>
    /// KDV oranı (%)
    /// </summary>
    public decimal TaxRate { get; set; }
    
    /// <summary>
    /// KDV tutarı (TRY)
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Toplam tutar (TRY)
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// İndirim oranı (%)
    /// </summary>
    public decimal DiscountRate { get; set; }
    
    /// <summary>
    /// İndirim tutarı (TRY)
    /// </summary>
    public decimal DiscountAmount { get; set; }
    
    /// <summary>
    /// GTİP kodu
    /// </summary>
    [MaxLength(20)]
    public string? GtipCode { get; set; }
    
    /// <summary>
    /// Mal/Hizmet kodu
    /// </summary>
    [MaxLength(20)]
    public string? ItemCode { get; set; }
}
