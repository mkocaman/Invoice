// Türkçe Açıklama:
// SandboxRunner veya başka servislerin göndereceği olay bildirimi DTO'su.

namespace WebApi.Contracts;

public sealed class InvoiceEventDto
{
    public string InvoiceId { get; set; } = default!;
    public string EventType { get; set; } = default!; // CREATED/SIGNED/SENT/ACK/NACK/DELIVERED/ERROR
    public string? StatusFrom { get; set; }
    public string? StatusTo { get; set; }
    public string? SystemCode { get; set; } // KZ/UZ/TR...
    public string? ExternalInvoiceNumber { get; set; }
    public string? EventKey { get; set; } // idempotency için
    public string? XmlPayload { get; set; }
    public string? JsonPayload { get; set; }
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public bool? Simulation { get; set; } // gelmezse env'den çıkarılır
    public DateTime? OccurredAtUtc { get; set; } // gelmezse now
    public string? Notes { get; set; }
}
