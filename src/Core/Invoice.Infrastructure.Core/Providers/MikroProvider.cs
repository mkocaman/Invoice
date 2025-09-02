using System.Net.Http.Json;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.Providers
{
    /// <summary>
    /// Mikro Yazılım e-Fatura (EMukellefSorgulamaV2, FaturaToEFaturaV2 vb. OpenAPI'ye göre).
    /// NOT: Bu sınıf TR sürümüyle çakışmaması için referans dışıdır. Kullanılmamalıdır.
    /// </summary>
    public partial class MikroProvider : IInvoiceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MikroProvider> _logger;
        public ProviderType ProviderType => ProviderType.MIKRO;

        public MikroProvider(IHttpClientFactory httpClientFactory, ILogger<MikroProvider> logger)
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
                    provider: ProviderType.MIKRO,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "Mikro için ApiBaseUrl tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: BaseUrl'i config'ten al, sabit/placeholder URL kullanma.
                var baseUrl = config.ApiBaseUrl.TrimEnd('/');
                var requestUri = $"{baseUrl}/Api/apiMethods/FaturaToEFaturaV2";

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
                        provider: ProviderType.MIKRO,
                        errorCode: $"HTTP_{(int)resp.StatusCode}",
                        errorMessage: $"Mikro servis hatası: {(int)resp.StatusCode} {resp.ReasonPhrase}. Yanıt: {body}"
                    );
                }

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.MIKRO,
                    providerReferenceNumber: null,
                    providerResponseMessage: "Mikro: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (HttpRequestException ex)
            {
                // Türkçe: DNS/SSL/ağ hatası – 502 benzeri durum, ama exception fırlatma yerine ProviderSendResult ile dön.
                return ProviderSendResult.Failed(
                    provider: ProviderType.MIKRO,
                    errorCode: "HTTP_REQUEST_EXCEPTION",
                    errorMessage: $"Mikro HTTP hatası: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.MIKRO,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"Mikro genel hata: {ex.Message}"
                );
            }
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config) => true;
    }
}
