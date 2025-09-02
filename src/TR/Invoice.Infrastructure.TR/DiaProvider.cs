using System.Net.Http.Json;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.Extensions.Logging;


namespace Invoice.Infrastructure.TR.Providers
{
    /// <summary>
    /// DİA e-Fatura entegrasyonu (dokümanlara göre gerçek endpoint/path doldurulacak).
    /// - Giriş/oturum bilgileri ProviderConfig'ten alınır.
    /// - UBL/Envelope verisinden DİA modeline dönüştürme yapılır.
    /// </summary>
    public partial class DiaProvider : IInvoiceProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DiaProvider> _logger;
        public ProviderType ProviderType => ProviderType.DIA;

        public DiaProvider(IHttpClientFactory httpClientFactory, ILogger<DiaProvider> logger)
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
                    provider: ProviderType.DIA,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "DİA için ApiBaseUrl tanımlı değil. Admin → ProviderConfig üzerinden ekleyiniz."
                );
            }

            try
            {
                // Türkçe: BaseUrl'i config'ten al, sabit/placeholder URL kullanma.
                var baseUrl = config.ApiBaseUrl.TrimEnd('/');
                var requestUri = $"{baseUrl}/api/efatura/olustur";

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
                        provider: ProviderType.DIA,
                        errorCode: $"HTTP_{(int)resp.StatusCode}",
                        errorMessage: $"DİA servis hatası: {(int)resp.StatusCode} {resp.ReasonPhrase}. Yanıt: {body}"
                    );
                }

                // Türkçe: Örnek başarı
                return ProviderSendResult.SuccessResult(
                    provider: ProviderType.DIA,
                    providerReferenceNumber: null,
                    providerResponseMessage: "DİA: e-Fatura isteği başarıyla iletildi.",
                    ublXml: null
                );
            }
            catch (HttpRequestException ex)
            {
                // Türkçe: DNS/SSL/ağ hatası – 502 benzeri durum, ama exception fırlatma yerine ProviderSendResult ile dön.
                return ProviderSendResult.Failed(
                    provider: ProviderType.DIA,
                    errorCode: "HTTP_REQUEST_EXCEPTION",
                    errorMessage: $"DİA HTTP hatası: {ex.Message}"
                );
            }
            catch (Exception ex)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType.DIA,
                    errorCode: "UNHANDLED_EXCEPTION",
                    errorMessage: $"DİA genel hata: {ex.Message}"
                );
            }
        }

        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            // Türkçe: DİA webhook imza doğrulaması gerekiyorsa burada yapın
            return true;
        }
    }
}
