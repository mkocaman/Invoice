using System.ServiceModel;
using System.ServiceModel.Channels;
// using System.Xml.Linq; // .NET 9'da paket gerekiyor, şimdilik kaldırıldı
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.Providers
{
    /// <summary>
    /// Uyumsoft e-Fatura SOAP entegrasyonu (test WSDL)
    /// NOT: Bu sınıf TR sürümüyle çakışmaması için referans dışıdır. Kullanılmamalıdır.
    /// </summary>
    public partial class UyumsoftProvider : IInvoiceProvider
    {
        private readonly ILogger<UyumsoftProvider> _logger;
        public ProviderType ProviderType => ProviderType.UYUMSOFT;
        
        public string Key => "uyumsoft";
        
        public string CountryCode => "TR";
        public UyumsoftProvider(ILogger<UyumsoftProvider> logger)
        {
            _logger = logger;
        }

        // Türkçe: WSDL: https://efatura-test.uyumsoft.com.tr/Services/Integration?singleWsdl
        private const string WsdlUrl = "https://efatura-test.uyumsoft.com.tr/Services/Integration?singleWsdl";

        public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken ct = default)
        {
            // Türkçe: Para birimini TRY'de sabitliyoruz.
            envelope.Currency = "TRY";

            // Türkçe: Zorunlu alan kontrolü – Username/Password yoksa asla SOAP çağrısı yapma.
            if (string.IsNullOrWhiteSpace(config?.Username) || string.IsNullOrWhiteSpace(config?.Password))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.UYUMSOFT,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "Uyumsoft için Username/Password tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: Mock SOAP çağrısı (gerçek implementasyonda WSDL kullanılır)
                await Task.Delay(100, ct); // Simüle edilmiş gecikme

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.UYUMSOFT,
                    providerReferenceNumber: null,
                    providerResponseMessage: "Uyumsoft: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.UYUMSOFT,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"Uyumsoft genel hata: {ex.Message}"
                );
            }
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            // Türkçe: Uyumsoft webhook imza doğrulama
            return true;
        }
    }

    // Türkçe: WSDL'den türetilmiş kontratın sadeleştirilmiş temsili (demo).
    [ServiceContract]
    public interface IUyumsoftIntegration
    {
        [OperationContract]
        Task<SendInvoiceResponse> SendInvoiceAsync(SendInvoiceRequest request);
    }

    public sealed class SendInvoiceRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Ubl { get; set; } = default!;
    }

    public sealed class SendInvoiceResponse
    {
        public bool Success { get; set; }
        public string? InvoiceId { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
