// Türkçe: Listeleme sorgu parametreleri (pagination + filtreleme + sıralama).
namespace WebApi.Contracts.Invoices
{
    public sealed class InvoiceQueryFilter
    {
        // Türkçe: Sayfa numarası (1'den başlar)
        public int Page { get; set; } = 1;

        // Türkçe: Sayfa boyutu
        public int PageSize { get; set; } = 20;

        // Türkçe: Sıralama ifadesi (örn: "createdAt desc" veya "amount asc")
        public string? Sort { get; set; }

        // Türkçe: Basit filtre örneği (örn: "status=Sent;currency=TRY")
        public string? Filter { get; set; }
    }
}
