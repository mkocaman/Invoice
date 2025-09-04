// Türkçe: Fatura gönderim iş akışı başlatma isteği (API DTO)
namespace WebApi.Contracts.Workflow
{
    public sealed class SubmitInvoiceRequest
    {
        // Türkçe: Müşteri kimliği (zorunlu)
        public string CustomerId { get; set; } = string.Empty;

        // Türkçe: Ülke kodu (TR/UZ/KZ)
        public string CountryCode { get; set; } = "TR";

        // Türkçe: Sağlayıcı tercihi (opsiyonel) — FORIBA, LOGO vb.
        public string? PreferProviderKey { get; set; }

        // Türkçe: Kapasite/özellik ihtiyacı — eInvoice, eArchive vs.
        public string Capability { get; set; } = "eInvoice";

        // Türkçe: Benzersiz idempotency anahtarı (istemci üretmeli)
        public string IdempotencyKey { get; set; } = string.Empty;

        // Türkçe: Basit örnek alanlar
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string? Description { get; set; }

        // Türkçe: Serileşmiş UBL/XML/JSON içeriği (demo amaçlı)
        public string? RawInvoicePayload { get; set; }
    }
}
