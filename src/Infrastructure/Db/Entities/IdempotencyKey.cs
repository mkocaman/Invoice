// Türkçe: İdempotensi anahtarları (API bazlı tekrar eden istekleri engeller)
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Db.Entities
{
    [Table("IdempotencyKeys")]
    public class IdempotencyKey
    {
        [Key] public long Id { get; set; }
        [MaxLength(128)] public string Key { get; set; } = string.Empty;   // İstemci gönderir
        [MaxLength(64)]  public string Scope { get; set; } = "Submit";     // Örn: Submit
        public string Response { get; set; } = string.Empty;               // İlk başarılı yanıtın snapshot'ı
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
