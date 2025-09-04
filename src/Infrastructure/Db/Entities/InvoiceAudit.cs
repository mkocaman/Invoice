// Türkçe Açıklama:
// Faturalara ait olay/durum ve ham payload kayıtları için audit tablosu.
// XML/JSON ham içerik, istek/yanıt gövdeleri, sistem (KZ/UZ), korelasyon bilgisi,
// simülasyon bayrağı ve SHA-256 özetleri dahil edilir.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities;

[Table("InvoiceAudits")]
public class InvoiceAudit
{
    [Key]
    public long Id { get; set; }

    // Uygulama içi fatura kimliği (GUID veya string olabilir)
    [MaxLength(64)]
    public string? InvoiceId { get; set; }

    // Harici sistemdeki fatura numarası
    [MaxLength(128)]
    public string? ExternalInvoiceNumber { get; set; }

    // Ör: CREATED, SIGNED, SENT, ACK, NACK, DELIVERED, ERROR
    [MaxLength(32)]
    public string EventType { get; set; } = default!;

    // Durum değişimi takibi (opsiyonel)
    [MaxLength(32)]
    public string? StatusFrom { get; set; }
    [MaxLength(32)]
    public string? StatusTo { get; set; }

    // Hangi ülke/sistem: "KZ" | "UZ" | "TR" ... vb.
    [MaxLength(8)]
    public string? SystemCode { get; set; }

    // Korelasyon/iz takibi
    [MaxLength(64)]
    public string? CorrelationId { get; set; }
    [MaxLength(64)]
    public string? TraceId { get; set; }

    // Ham içerikler (TEXT). Çok büyük içerikler için sıkıştırma ya da parçalama ileride eklenebilir.
    public string? XmlPayload { get; set; }      // Faturanın üretilen XML'i (varsa)
    public string? JsonPayload { get; set; }     // Alternatif JSON (varsa)

    public string? RequestBody { get; set; }     // Harici sisteme gönderilen istek gövdesi (maskelenmiş!)
    public string? ResponseBody { get; set; }    // Harici sistemden gelen yanıt gövdesi (maskelenmiş!)

    // Güvenlik / Maskeleme notu
    [MaxLength(256)]
    public string? RedactionNotes { get; set; }  // Hangi alanlar maske/çıkarıldı

    // İçerik özetleri (bütünlük kanıtı)
    [MaxLength(64)]
    public string? XmlSha256 { get; set; }
    [MaxLength(64)]
    public string? JsonSha256 { get; set; }
    [MaxLength(64)]
    public string? RequestSha256 { get; set; }
    [MaxLength(64)]
    public string? ResponseSha256 { get; set; }

    // Simülasyon bayrağı (SandboxRunner'dan veya appsettings'ten gelebilir)
    public bool Simulation { get; set; }

    // Kullanıcı/servis kimliği (opsiyonel)
    [MaxLength(128)]
    public string? Actor { get; set; }

    // Zaman damgaları
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    // Ek açıklama/diagnostic
    [MaxLength(512)]
    public string? Notes { get; set; }
}
