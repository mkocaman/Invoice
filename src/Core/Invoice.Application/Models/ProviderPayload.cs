namespace Invoice.Application.Models
{
    /// <summary>
    /// Türkçe: Sağlayıcıya gönderilecek minimal taşıyıcı model (REST/SOAP);
    /// JSON, UBL XML vb. içerikleri taşımak için.
    /// </summary>
    public class ProviderPayload
    {
        public string TenantId { get; set; } = default!;
        public string ProviderKey { get; set; } = default!;
        public string? JsonBody { get; set; }       // Türkçe: JSON içerik (varsa)
        public string? UblXml { get; set; }         // Türkçe: UBL XML düz metin (base64 değil)
        public string? AdditionalInfo { get; set; } // Türkçe: Ek bilgiler (firma kodu, yıl vb.)
    }
}
