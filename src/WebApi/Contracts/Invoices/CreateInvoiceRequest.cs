// Türkçe: Fatura oluşturma isteği DTO'su (örnek alanlar). Gerçek alanları domain'e göre genişletin.
namespace WebApi.Contracts.Invoices
{
    public sealed class CreateInvoiceRequest
    {
        // Türkçe: Müşteri numarası (zorunlu)
        public string CustomerId { get; set; } = string.Empty;

        // Türkçe: Para birimi (örn: "TRY", "USD")
        public string Currency { get; set; } = "TRY";

        // Türkçe: Toplam tutar
        public decimal Amount { get; set; }

        // Türkçe: Açıklama (opsiyonel)
        public string? Description { get; set; }
    }
}
