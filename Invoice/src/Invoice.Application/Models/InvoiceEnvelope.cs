namespace Invoice.Application.Models;

/// <summary>
/// Fatura zarfı - entegratöre gönderilecek tüm fatura verilerini içerir
/// </summary>
public class InvoiceEnvelope
{
    /// <summary>
    /// Fatura ID'si
    /// </summary>
    public Guid InvoiceId { get; set; }
    
    /// <summary>
    /// Fatura numarası
    /// </summary>
    public string InvoiceNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Fatura tarihi
    /// </summary>
    public DateTime InvoiceDate { get; set; }
    
    /// <summary>
    /// Vade tarihi
    /// </summary>
    public DateTime DueDate { get; set; }
    
    /// <summary>
    /// Para birimi (TRY)
    /// </summary>
    public string Currency { get; set; } = "TRY";
    
    /// <summary>
    /// Döviz kuru (1.0 = TRY)
    /// </summary>
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    /// <summary>
    /// Müşteri bilgileri
    /// </summary>
    public CustomerInfo Customer { get; set; } = new();
    
    /// <summary>
    /// Satır kalemleri
    /// </summary>
    public List<InvoiceLineItem> LineItems { get; set; } = new();
    
    /// <summary>
    /// Toplam tutar (KDV hariç)
    /// </summary>
    public decimal SubTotal { get; set; }
    
    /// <summary>
    /// KDV tutarı
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Genel toplam (KDV dahil)
    /// </summary>
    public decimal TotalAmount { get; set; }
    
    /// <summary>
    /// EŞÜ (Elektronik Şart Ücreti) tutarı
    /// </summary>
    public decimal EshuAmount { get; set; }
    
    /// <summary>
    /// EŞÜ açıklaması
    /// </summary>
    public string? EshuDescription { get; set; }
    
    /// <summary>
    /// Oturum bilgileri (şarj istasyonu, oturum ID'si vb.)
    /// </summary>
    public SessionInfo? Session { get; set; }
    
    /// <summary>
    /// Ek notlar
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Etiketler (arama/filtreleme için)
    /// </summary>
    public List<string> Tags { get; set; } = new();
}

/// <summary>
/// Müşteri bilgileri
/// </summary>
public class CustomerInfo
{
    /// <summary>
    /// Müşteri ID'si
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Müşteri adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// VKN/TCKN
    /// </summary>
    public string? TaxNumber { get; set; }
    
    /// <summary>
    /// Vergi dairesi
    /// </summary>
    public string? TaxOffice { get; set; }
    
    /// <summary>
    /// Adres
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Telefon
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// E-posta
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Ülke kodu (TR)
    /// </summary>
    public string CountryCode { get; set; } = "TR";
}

/// <summary>
/// Fatura satır kalemi
/// </summary>
public class InvoiceLineItem
{
    /// <summary>
    /// Satır ID'si
    /// </summary>
    public Guid LineId { get; set; }
    
    /// <summary>
    /// Ürün/hizmet adı
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Miktar
    /// </summary>
    public decimal Quantity { get; set; }
    
    /// <summary>
    /// Birim fiyat (KDV hariç)
    /// </summary>
    public decimal UnitPrice { get; set; }
    
    /// <summary>
    /// Satır toplamı (KDV hariç)
    /// </summary>
    public decimal LineTotal { get; set; }
    
    /// <summary>
    /// KDV oranı (%)
    /// </summary>
    public decimal TaxRate { get; set; }
    
    /// <summary>
    /// KDV tutarı
    /// </summary>
    public decimal TaxAmount { get; set; }
    
    /// <summary>
    /// Birim (kWh, adet, saat vb.)
    /// </summary>
    public string Unit { get; set; } = "adet";
    
    /// <summary>
    /// Ürün kodu
    /// </summary>
    public string? ProductCode { get; set; }
}

/// <summary>
/// Oturum bilgileri (şarj istasyonu)
/// </summary>
public class SessionInfo
{
    /// <summary>
    /// Oturum ID'si
    /// </summary>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// İstasyon ID'si
    /// </summary>
    public string StationId { get; set; } = string.Empty;
    
    /// <summary>
    /// İstasyon adı
    /// </summary>
    public string StationName { get; set; } = string.Empty;
    
    /// <summary>
    /// Başlangıç zamanı
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Bitiş zamanı
    /// </summary>
    public DateTime EndTime { get; set; }
    
    /// <summary>
    /// Toplam enerji (kWh)
    /// </summary>
    public decimal TotalEnergy { get; set; }
    
    /// <summary>
    /// Ortalama güç (kW)
    /// </summary>
    public decimal AveragePower { get; set; }
    
    /// <summary>
    /// Maksimum güç (kW)
    /// </summary>
    public decimal MaxPower { get; set; }
}
