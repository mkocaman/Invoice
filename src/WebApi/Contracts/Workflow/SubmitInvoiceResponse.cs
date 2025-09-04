// Türkçe: Fatura gönderim iş akışı yanıtı
namespace WebApi.Contracts.Workflow
{
    public sealed class SubmitInvoiceResponse
    {
        public string WorkflowId { get; set; } = string.Empty; // Türkçe: İş akışı ID (GUID)
        public string InvoiceId { get; set; } = string.Empty;  // Türkçe: Sistem içi fatura ID
        public string Status { get; set; } = "Queued";         // Türkçe: İlk durum
        public string? ProviderKey { get; set; }               // Türkçe: Seçilen sağlayıcı
    }
}
