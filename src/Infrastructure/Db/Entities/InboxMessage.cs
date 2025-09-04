// Türkçe: Inbox mesajları (alınan olayların idempotent işlenmesi)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities
{
    [Table("InboxMessages")]
    public class InboxMessage
    {
        [Key] public long Id { get; set; }
        [MaxLength(128)] public string MessageId { get; set; } = string.Empty;     // MQ message-id / delivery tag
        [MaxLength(64)] public string Type { get; set; } = string.Empty;           // Örn: Provider.Callback
        public string Payload { get; set; } = string.Empty;                        // JSON
        public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAtUtc { get; set; }
        public bool Succeeded { get; set; }
        [MaxLength(256)] public string? Error { get; set; }
    }
}
