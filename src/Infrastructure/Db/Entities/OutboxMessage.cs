// Türkçe: Outbox mesajları (gönderilecek olaylar)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities
{
    [Table("OutboxMessages")]
    public class OutboxMessage
    {
        [Key] public long Id { get; set; }
        [MaxLength(64)] public string AggregateId { get; set; } = string.Empty; // workflowId veya invoiceId
        [MaxLength(64)] public string Type { get; set; } = string.Empty;        // Örn: Invoice.Submitted
        public string Payload { get; set; } = string.Empty;                     // JSON
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? SentAtUtc { get; set; }
        public int Attempt { get; set; } = 0;
        public bool Locked { get; set; } = false;
        public DateTime? LockedUntilUtc { get; set; }
    }
}
