using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// Rapor ID entity'si (7-gün rejimi)
/// </summary>
public class ReportId : BaseEntity
{
    /// <summary>
    /// Rapor ID numarası
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ReportNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Rapor başlangıç tarihi
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Rapor bitiş tarihi
    /// </summary>
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Rapor dönemi (7-gün)
    /// </summary>
    public int PeriodDays { get; set; } = 7;
    
    /// <summary>
    /// Rapor durumu
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Rapor oluşturulma tarihi
    /// </summary>
    public DateTime GeneratedDate { get; set; }
    
    /// <summary>
    /// Rapor gönderilme tarihi
    /// </summary>
    public DateTime? SentDate { get; set; }
    
    /// <summary>
    /// Rapor onaylanma tarihi
    /// </summary>
    public DateTime? ApprovedDate { get; set; }
    
    /// <summary>
    /// Rapor reddedilme tarihi
    /// </summary>
    public DateTime? RejectedDate { get; set; }
    
    /// <summary>
    /// Rapor red nedeni
    /// </summary>
    [MaxLength(1000)]
    public string? RejectionReason { get; set; }
    
    /// <summary>
    /// Rapor notları
    /// </summary>
    [MaxLength(2000)]
    public string? Notes { get; set; }
}
