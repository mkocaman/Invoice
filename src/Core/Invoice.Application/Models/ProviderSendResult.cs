using Invoice.Domain.Enums;

namespace Invoice.Application.Models
{
    /// <summary>
    /// Türkçe: Sağlayıcıya (entegratöre) gönderim sonucunu temsil eder.
    /// </summary>
    public record ProviderSendResult(
        bool Success,
        ProviderType Provider,
        string? ProviderReferenceNumber,
        string? ProviderResponseMessage,
        string? UblXml,
        string? ErrorCode,
        string? ErrorMessage)
    {
        /// <summary>
        /// Türkçe: Başarılı sonuç üretir.
        /// </summary>
        public static ProviderSendResult SuccessResult(
            ProviderType provider,
            string? providerReferenceNumber,
            string? providerResponseMessage,
            string? ublXml)
            => new(true, provider, providerReferenceNumber, providerResponseMessage, ublXml, null, null);

        /// <summary>
        /// Türkçe: Hatalı sonuç üretir.
        /// </summary>
        public static ProviderSendResult Failed(
            ProviderType provider,
            string? errorCode,
            string? errorMessage)
            => new(false, provider, null, null, null, errorCode, errorMessage);
    }
}
