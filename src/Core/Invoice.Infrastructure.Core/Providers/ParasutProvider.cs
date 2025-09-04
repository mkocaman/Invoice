// Türkçe: Parasut e-Fatura entegrasyonu
using System.Net.Http.Json;
using System.Text.Json;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Infrastructure.Providers.Http;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.Providers
{
    /// <summary>
    /// Paraşüt e-Fatura REST entegrasyonu (OAuth2 Client Credentials)
    /// NOT: Bu sınıf TR sürümüyle çakışmaması için referans dışıdır. Kullanılmamalıdır.
    /// </summary>
    public partial class ParasutProvider : IInvoiceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ParasutProvider> _logger;
        public ProviderType ProviderType => ProviderType.PARASUT;
        
        public string Key => "parasut";
        
        public string CountryCode => "TR";
        public ParasutProvider(IHttpClientFactory httpClientFactory, ILogger<ParasutProvider> logger)
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
                    provider: ProviderType.PARASUT,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "Paraşüt için ApiBaseUrl tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: BaseUrl'i config'ten al, sabit/placeholder URL kullanma.
                var baseUrl = config.ApiBaseUrl.TrimEnd('/');
                var requestUri = $"{baseUrl}/api/v4/sales_invoices";

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
                        provider: ProviderType.PARASUT,
                        errorCode: $"HTTP_{(int)resp.StatusCode}",
                        errorMessage: $"Paraşüt servis hatası: {(int)resp.StatusCode} {resp.ReasonPhrase}. Yanıt: {body}"
                    );
                }

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.PARASUT,
                    providerReferenceNumber: null,
                    providerResponseMessage: "Paraşüt: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (HttpRequestException ex)
            {
                // Türkçe: DNS/SSL/ağ hatası – 502 benzeri durum, ama exception fırlatma yerine ProviderSendResult ile dön.
                return ProviderSendResult.Failed(
                    provider: ProviderType.PARASUT,
                    errorCode: "HTTP_REQUEST_EXCEPTION",
                    errorMessage: $"Paraşüt HTTP hatası: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.PARASUT,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"Paraşüt genel hata: {ex.Message}"
                );
            }
        }

        private async Task<string> GetAccessTokenAsync(ProviderConfig config, CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient("parasut");
            var tokenBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", config.GrantType ?? "client_credentials"),
                new KeyValuePair<string, string>("client_id", config.ClientId!),
                new KeyValuePair<string, string>("client_secret", config.ClientSecret!)
            });

            var resp = await client.PostAsync(config.TokenUrl, tokenBody, ct);
            var content = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Paraşüt token alma hatası: {content}");

            // TODO: JSON parse ile access_token çek
            return "mock_access_token";
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            // Türkçe: Paraşüt webhook imza doğrulama dokümana göre eklenebilir.
            return true;
        }
    }
}
