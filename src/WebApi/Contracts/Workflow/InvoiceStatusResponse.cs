// Türkçe: İş akışı durum sorgu yanıtı
namespace WebApi.Contracts.Workflow
{
    public sealed class InvoiceStatusResponse
    {
        public string WorkflowId { get; set; } = string.Empty;
        public string InvoiceId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Draft/Submitted/Sent/Accepted/Rejected/Error
        public string? ProviderKey { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastUpdatedUtc { get; set; }
        public string? LastError { get; set; }
    }
}
