using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

/// <summary>
/// EŞÜ v3 entity (E-Şarj Ünitesi)
/// </summary>
public class Eshu : BaseEntity
{
    /// <summary>
    /// EŞÜ seri numarası
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ modeli
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ markası
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ tipi
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ gücü (kW)
    /// </summary>
    public decimal Power { get; set; }
    
    /// <summary>
    /// EŞÜ voltajı (V)
    /// </summary>
    public int Voltage { get; set; }
    
    /// <summary>
    /// EŞÜ akımı (A)
    /// </summary>
    public decimal Current { get; set; }
    
    /// <summary>
    /// EŞÜ frekansı (Hz)
    /// </summary>
    public int Frequency { get; set; }
    
    /// <summary>
    /// EŞÜ üretici firma
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Manufacturer { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ üretim tarihi
    /// </summary>
    public DateTime ManufacturingDate { get; set; }
    
    /// <summary>
    /// EŞÜ kurulum tarihi
    /// </summary>
    public DateTime InstallationDate { get; set; }
    
    /// <summary>
    /// EŞÜ lokasyonu (adres)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Location { get; set; } = string.Empty;
    
    /// <summary>
    /// EŞÜ koordinatları (enlem)
    /// </summary>
    public decimal Latitude { get; set; }
    
    /// <summary>
    /// EŞÜ koordinatları (boylam)
    /// </summary>
    public decimal Longitude { get; set; }
    
    /// <summary>
    /// EŞÜ durumu (aktif/pasif)
    /// </summary>
    public new bool IsActive { get; set; } = true;
    
    /// <summary>
    /// EŞÜ son bakım tarihi
    /// </summary>
    public DateTime? LastMaintenanceDate { get; set; }
    
    /// <summary>
    /// EŞÜ sonraki bakım tarihi
    /// </summary>
    public DateTime? NextMaintenanceDate { get; set; }
}
