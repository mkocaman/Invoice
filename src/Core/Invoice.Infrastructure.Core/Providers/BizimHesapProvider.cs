using System.Net.Http.Headers;
using System.Net.Http.Json;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Invoice.Infrastructure.Providers
{
    /// <summary>
    /// BizimHesap B2B API – addinvoice uç noktası (GitBook dokümana göre).
    /// NOT: Bu sınıf TR sürümüyle çakışmaması için referans dışıdır. Kullanılmamalıdır.
    /// </summary>
    public partial class BizimHesapProvider : IInvoiceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BizimHesapProvider> _logger;
        public ProviderType ProviderType => ProviderType.BIZIMHESAP;

        public BizimHesapProvider(IHttpClientFactory httpClientFactory, ILogger<BizimHesapProvider> logger)
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
                    provider: ProviderType.BIZIMHESAP,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "BizimHesap için ApiBaseUrl tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: BaseUrl'i config'ten al, sabit/placeholder URL kullanma.
                var baseUrl = config.ApiBaseUrl.TrimEnd('/');
                var requestUri = $"{baseUrl}/api/b2b/addinvoice";

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
                        provider: ProviderType.BIZIMHESAP,
                        errorCode: $"HTTP_{(int)resp.StatusCode}",
                        errorMessage: $"BizimHesap servis hatası: {(int)resp.StatusCode} {resp.ReasonPhrase}. Yanıt: {body}"
                    );
                }

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.BIZIMHESAP,
                    providerReferenceNumber: null,
                    providerResponseMessage: "BizimHesap: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (HttpRequestException ex)
            {
                // Türkçe: DNS/SSL/ağ hatası – 502 benzeri durum, ama exception fırlatma yerine ProviderSendResult ile dön.
                return ProviderSendResult.Failed(
                    provider: ProviderType.BIZIMHESAP,
                    errorCode: "HTTP_REQUEST_EXCEPTION",
                    errorMessage: $"BizimHesap HTTP hatası: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.BIZIMHESAP,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"BizimHesap genel hata: {ex.Message}"
                );
            }
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config) => true;
    }
}
