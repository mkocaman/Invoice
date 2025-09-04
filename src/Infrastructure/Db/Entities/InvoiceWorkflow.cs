// Türkçe: Fatura iş akışı ana tablosu
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities
{
    [Table("InvoiceWorkflows")]
    public class InvoiceWorkflow
    {
        [Key]
        public Guid Id { get; set; }
        [MaxLength(64)]
        public string InvoiceId { get; set; } = string.Empty;
        [MaxLength(2)]
        public string CountryCode { get; set; } = "TR";
        [MaxLength(32)]
        public string? ProviderKey { get; set; }
        [MaxLength(32)]
        public string Status { get; set; } = "Submitted"; // Draft/Submitted/Sent/Accepted/Rejected/Error
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedUtc { get; set; }
        [MaxLength(256)]
        public string? LastError { get; set; }
    }
}
