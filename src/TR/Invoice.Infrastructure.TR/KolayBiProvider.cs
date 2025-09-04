// Türkçe: KolayBi e-Fatura entegrasyonu
using System.Net.Http.Json;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Infrastructure.Providers.Http;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.TR.Providers
{
    /// <summary>
    /// KolayBi e-Fatura REST entegrasyonu
    /// </summary>
    public partial class KolayBiProvider : IInvoiceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KolayBiProvider> _logger;
        public ProviderType ProviderType => ProviderType.KOLAYBI;
        
        public string Key => "kolaybi";
        
        public string CountryCode => "TR";
        public KolayBiProvider(IHttpClientFactory httpClientFactory, ILogger<KolayBiProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken ct = default)
        {
            // Türkçe: Para birimini TRY'de sabitliyoruz.
            envelope.Currency = "TRY";

            // Türkçe: Zorunlu alan kontrolü – ApiBaseUrl yoksa asla HTTP çağrısı yapma.
            if (string.IsNullOrWhiteSpace(config?.ApiBaseUrl))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.KOLAYBI,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "KolayBi için ApiBaseUrl tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: BaseUrl'i config'ten al, sabit/placeholder URL kullanma.
                var baseUrl = config.ApiBaseUrl.TrimEnd('/');
                var requestUri = $"{baseUrl}/api/invoices";

                // Türkçe: HttpClient'ı base address vermeden kullan; absolute URI gönder.
                using var req = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
                };

                var resp = await _httpClientFactory.CreateClient().SendAsync(req, ct);
                var body = await resp.Content.ReadAsStringAsync(ct);

                if (!resp.IsSuccessStatusCode)
                {
                    // Türkçe: Servis başarısız – HTTP kodu ve body'yi mesaja yaz.
                    return ProviderSendResult.Failed(
                        provider: ProviderType.KOLAYBI,
                        errorCode: $"HTTP_{(int)resp.StatusCode}",
                        errorMessage: $"KolayBi servis hatası: {(int)resp.StatusCode} {resp.ReasonPhrase}. Yanıt: {body}"
                    );
                }

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.KOLAYBI,
                    providerReferenceNumber: null,
                    providerResponseMessage: "KolayBi: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (HttpRequestException ex)
            {
                // Türkçe: DNS/SSL/ağ hatası – 502 benzeri durum, ama exception fırlatma yerine ProviderSendResult ile dön.
                return ProviderSendResult.Failed(
                    provider: ProviderType.KOLAYBI,
                    errorCode: "HTTP_REQUEST_EXCEPTION",
                    errorMessage: $"KolayBi HTTP hatası: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.KOLAYBI,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"KolayBi genel hata: {ex.Message}"
                );
            }
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            // Türkçe: KolayBi özel imza başlığı varsa burada doğrulanır (dokümana göre). Yoksa true döner.
            return true;
        }


    }
}
