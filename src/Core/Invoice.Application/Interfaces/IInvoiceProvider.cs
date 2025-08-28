using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Application.Interfaces
{
    /// <summary>
    /// Türkçe: Her entegrasyon sağlayıcısı bu arayüzü uygular.
    /// </summary>
    public interface IInvoiceProvider
    {
        /// <summary>Türkçe: Sağlayıcı tipini döndürür (Foriba, Logo, Mikro vb.).</summary>
        ProviderType ProviderType { get; }

        /// <summary>
        /// Türkçe: Faturayı sağlayıcıya gönderir.
        /// </summary>
        Task<ProviderSendResult> SendInvoiceAsync(
            InvoiceEnvelope envelope,          // Türkçe: Gönderilecek zarf (UBL veya JSON gövdeyi oluşturmak için)
            ProviderConfig config,             // Türkçe: Tenant'a özel sağlayıcı ayarları
            CancellationToken ct = default);   // Türkçe: İptal belirteci

        /// <summary>
        /// Türkçe: Webhook imzasını doğrular (varsa).
        /// </summary>
        bool VerifyWebhookSignature(
            IReadOnlyDictionary<string, string> headers,
            string body,
            ProviderConfig config);
            
        /// <summary>
        /// Türkçe: Bu provider'ın belirtilen ülkeyi destekleyip desteklemediğini kontrol eder.
        /// </summary>
        bool SupportsCountry(string countryCode);
        
        /// <summary>
        /// Türkçe: Bu provider'ın belirtilen provider tipini destekleyip desteklemediğini kontrol eder.
        /// </summary>
        bool Supports(ProviderType type);
    }
}
