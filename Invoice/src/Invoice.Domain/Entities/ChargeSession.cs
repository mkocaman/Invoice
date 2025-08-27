using System.ComponentModel.DataAnnotations;
using Invoice.Domain.Enums;
using Invoice.Domain.ValueObjects;

namespace Invoice.Domain.Entities;

/// <summary>
/// Şarj seansı entity'si
/// </summary>
public class ChargeSession : BaseEntity
{
    /// <summary>
    /// EŞÜ ID
    /// </summary>
    public Guid EshuId { get; set; }
    
    /// <summary>
    /// EŞÜ navigation property
    /// </summary>
    public virtual Eshu Eshu { get; set; } = null!;
    
    /// <summary>
    /// Şarj seansı başlangıç tarihi
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Şarj seansı bitiş tarihi
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Şarj süresi (dakika)
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Şarj edilen enerji miktarı (kWh)
    /// </summary>
    public decimal EnergyConsumed { get; set; }
    
    /// <summary>
    /// Şarj ücreti (TRY)
    /// </summary>
    public decimal ChargeAmount { get; set; }
    
    /// <summary>
    /// Şarj gücü (kW)
    /// </summary>
    public decimal ChargePower { get; set; }
    
    /// <summary>
    /// Şarj akımı (A)
    /// </summary>
    public decimal ChargeCurrent { get; set; }
    
    /// <summary>
    /// Şarj voltajı (V)
    /// </summary>
    public int ChargeVoltage { get; set; }
    
    /// <summary>
    /// Şarj frekansı (Hz)
    /// </summary>
    public int ChargeFrequency { get; set; }
    
    /// <summary>
    /// Şarj sıcaklığı (°C)
    /// </summary>
    public decimal? ChargeTemperature { get; set; }
    
    /// <summary>
    /// Şarj durumu
    /// </summary>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// Şarj seansı notları
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    /// <summary>
    /// Şarj seansı tamamlandı mı?
    /// </summary>
    public bool IsCompleted { get; set; }
    
    /// <summary>
    /// Şarj seansı iptal edildi mi?
    /// </summary>
    public bool IsCancelled { get; set; }
    
    /// <summary>
    /// İptal nedeni
    /// </summary>
    [MaxLength(500)]
    public string? CancellationReason { get; set; }
}
