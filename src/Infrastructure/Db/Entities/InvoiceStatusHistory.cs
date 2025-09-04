// Türkçe Açıklama:
// Fatura durum geçişlerinin hızlı raporlanması için normalleştirilmiş tablo.
// Audit büyük gövdeyi tutar; StatusHistory sadece durum + zaman + latency odaklıdır.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities;

[Table("InvoiceStatusHistory")]
public class InvoiceStatusHistory
{
    [Key]
    public long Id { get; set; }

    [MaxLength(64)]
    public string InvoiceId { get; set; } = default!; // Uygulama içi kimlik

    [MaxLength(128)]
    public string? ExternalInvoiceNumber { get; set; } // Harici sistemdeki numara/id

    // Örn: CREATED, SIGNED, SENT, ACK, NACK, DELIVERED, ERROR
    [MaxLength(32)]
    public string EventType { get; set; } = default!;

    // Durum değişimi (opsiyonel)
    [MaxLength(32)]
    public string? StatusFrom { get; set; }
    [MaxLength(32)]
    public string? StatusTo { get; set; }

    // Hangi ülke/sistem: "KZ" | "UZ" | "TR" vb.
    [MaxLength(8)]
    public string? SystemCode { get; set; }

    // Olay zaman damgası
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    // Bir önceki olaydan bu olaya kadar geçen süre (ms)
    public long? LatencyMs { get; set; }

    // Idempotency için olay anahtarı (örn. sistemden gelen eventId)
    [MaxLength(128)]
    public string? EventKey { get; set; }

    // Simülasyon mu?
    public bool Simulation { get; set; }
}
