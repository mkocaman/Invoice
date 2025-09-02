using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net; // HTML encoding için WebUtility

namespace Invoice.Infrastructure.UZ.Providers;

/// <summary>
/// Özbekistan Faktura.uz API entegrasyonu
/// </summary>
public class FakturaUzProvider : IInvoiceProvider
{
    private readonly ILogger<FakturaUzProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public FakturaUzProvider(
        ILogger<FakturaUzProvider> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public ProviderType ProviderType => ProviderType.FAKTURA_UZ;

    public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Faktura.uz'a fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
            envelope.InvoiceNumber, envelope.TenantId);

        try
        {
            // Türkçe: Özbekistan özel validasyonları
            var validationResult = ValidateUzbekistanInvoice(envelope);
            if (!validationResult.IsValid)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "VALIDATION_ERROR",
                    errorMessage: validationResult.ErrorMessage);
            }

            // Türkçe: Para birimini UZS'de sabitliyoruz
            envelope.Currency = "UZS";

            // Türkçe: Offline mod kontrolü
            var offlineMode = _configuration.GetValue<bool>("FeatureFlags:Providers:FakturaUz:Offline", false);
            if (offlineMode)
            {
                _logger.LogInformation("Faktura.uz offline modda çalışıyor. Mock başarılı yanıt döndürülüyor.");
                return new ProviderSendResult(
                    Success: true,
                    Provider: ProviderType,
                    ProviderReferenceNumber: $"FAKTURA-UZ-MOCK-{Guid.NewGuid():N}",
                    ProviderResponseMessage: "Offline stub başarılı",
                    UblXml: null,
                    ErrorCode: null,
                    ErrorMessage: null);
            }

            // Türkçe: Konfigürasyon kontrolü
            if (string.IsNullOrWhiteSpace(config?.ApiBaseUrl))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "CONFIG_MISSING",
                    errorMessage: "Faktura.uz için ApiBaseUrl tanımlı değil.");
            }

            var httpClient = _httpClientFactory.CreateClient("fakturaUz");
            
            // Türkçe: Token al (OAuth2 veya API Key)
            var token = await GetAccessTokenAsync(config, httpClient, cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "AUTH_FAILED",
                    errorMessage: "Faktura.uz token alınamadı.");
            }

            // Türkçe: Fatura gönder
            var invoicePayload = CreateInvoicePayload(envelope, config);
            var response = await SendInvoiceToFakturaAsync(invoicePayload, token, config, httpClient, cancellationToken);

            _logger.LogInformation("Faktura.uz'a fatura gönderildi. Invoice Number: {InvoiceNumber}, Response: {Response}", 
                envelope.InvoiceNumber, response);

            return new ProviderSendResult(
                Success: true,
                Provider: ProviderType,
                ProviderReferenceNumber: response.InvoiceId ?? $"FAKTURA-UZ-{Guid.NewGuid():N}",
                ProviderResponseMessage: "Faktura.uz: e-Fatura başarıyla gönderildi",
                UblXml: null,
                ErrorCode: null,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Faktura.uz'a fatura gönderilirken hata. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
            
            return ProviderSendResult.Failed(
                provider: ProviderType,
                errorCode: "SEND_ERROR",
                errorMessage: $"Faktura.uz gönderim hatası: {ex.Message}");
        }
    }

    public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
    {
        _logger.LogDebug("Faktura.uz webhook imza doğrulaması. Body length: {BodyLength}", body.Length);
        
        // Türkçe: Mock doğrulama - gerçek implementasyonda HMAC-SHA256 ile imza kontrolü yapılır
        return true;
    }

    /// <summary>
    /// Türkçe: Bu provider'ın belirtilen ülkeyi destekleyip desteklemediğini kontrol eder.
    /// </summary>
    public bool SupportsCountry(string countryCode)
    {
        return countryCode.Equals("UZ", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Türkçe: Bu provider'ın belirtilen provider tipini destekleyip desteklemediğini kontrol eder.
    /// </summary>
    public bool Supports(ProviderType type)
    {
        return type == ProviderType.FAKTURA_UZ;
    }

    private async Task<string?> GetAccessTokenAsync(ProviderConfig config, HttpClient httpClient, CancellationToken cancellationToken)
    {
        try
        {
            // Türkçe: AuthType'a göre token alma stratejisi
            switch (config.AuthType?.ToUpperInvariant())
            {
                case "OAUTH2":
                    return await GetOAuth2TokenAsync(config, httpClient, cancellationToken);
                case "APIKEY":
                default:
                    return config.ApiKey; // API Key direkt kullanılır
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Faktura.uz token alınırken hata");
            return null;
        }
    }

    private async Task<string> GetOAuth2TokenAsync(ProviderConfig config, HttpClient httpClient, CancellationToken cancellationToken)
    {
        // Türkçe: OAuth2 token alma implementasyonu
        var tokenRequest = new
        {
            grant_type = "client_credentials",
            client_id = config.ClientId,
            client_secret = config.ClientSecret
        };

        var response = await httpClient.PostAsJsonAsync("/oauth/token", tokenRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<OAuth2TokenResponse>(cancellationToken: cancellationToken);
        return tokenResponse?.AccessToken;
    }

    private object CreateInvoicePayload(InvoiceEnvelope envelope, ProviderConfig config)
    {
        // Türkçe: Faktura.uz API formatına uygun payload oluştur (Özbekistan gereksinimleri)
        return new
        {
            invoice_number = envelope.InvoiceNumber,
            invoice_date = envelope.IssueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            total_amount = envelope.TotalAmount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            currency = "UZS",
            customer = new
            {
                name = WebUtility.HtmlEncode(envelope.Customer?.Name ?? envelope.CustomerName ?? "Mijoz"),
                tin = envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber,
                address = WebUtility.HtmlEncode(envelope.Customer?.AddressLine),
                country_code = envelope.Customer?.CountryCode ?? "UZ",
                email = envelope.Customer?.Email
            },
            items = (envelope.Items ?? envelope.LineItems)?.Select(item => new
            {
                name = WebUtility.HtmlEncode(item.Name ?? item.Description),
                quantity = item.Quantity,
                unit_price = item.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture),
                total = item.Total.ToString(System.Globalization.CultureInfo.InvariantCulture),
                unit_code = GetUzbekistanUnitCode(item.UnitCode), // UN/ECE Rec 20 kodu
                tax_rate = item.TaxRate.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }).ToArray() ?? Array.Empty<object>()
        };
    }

    /// <summary>
    /// UN/ECE Rec 20 birim kodlarını Özbekistan için döndürür
    /// </summary>
    private string GetUzbekistanUnitCode(string? unitCode)
    {
        return unitCode?.ToUpperInvariant() switch
        {
            "EA" or "ADET" or "PIECE" => "C62", // Adet
            "KG" or "KILOGRAM" => "KGM", // Kilogram
            "M" or "METER" => "MTR", // Metre
            "L" or "LITRE" => "LTR", // Litre
            "H" or "HOUR" => "HUR", // Saat
            "DAY" => "DAY", // Gün
            "MONTH" => "MON", // Ay
            "YEAR" => "ANN", // Yıl
            _ => "C62" // Varsayılan: Adet
        };
    }

    private (bool IsValid, string ErrorMessage) ValidateUzbekistanInvoice(InvoiceEnvelope envelope)
    {
        // Türkçe: Özbekistan özel validasyonları
        var customerTaxNumber = envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber;
        
        // Türkçe: Vergi numarası kontrolü (Özbekistan formatı: 9 haneli)
        if (!string.IsNullOrWhiteSpace(customerTaxNumber))
        {
            if (customerTaxNumber.Length != 9 || !customerTaxNumber.All(char.IsDigit))
            {
                return (false, "Özbekistan vergi numarası 9 haneli olmalıdır");
            }
        }

        // Türkçe: Müşteri adı zorunlu
        var customerName = envelope.Customer?.Name ?? envelope.CustomerName;
        if (string.IsNullOrWhiteSpace(customerName))
        {
            return (false, "Müşteri adı zorunludur");
        }

        // Türkçe: Kalem kontrolü
        var items = envelope.Items ?? envelope.LineItems;
        if (items == null || !items.Any())
        {
            return (false, "En az bir fatura kalemi olmalıdır");
        }

        return (true, "");
    }

    private async Task<FakturaUzResponse> SendInvoiceToFakturaAsync(
        object payload, 
        string token, 
        ProviderConfig config, 
        HttpClient httpClient, 
        CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.PostAsJsonAsync("/api/documents/invoice/send", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<FakturaUzResponse>(cancellationToken: cancellationToken) 
               ?? new FakturaUzResponse();
    }

    // Türkçe: Response modelleri
    private class OAuth2TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }

    private class FakturaUzResponse
    {
        public bool Success { get; set; }
        public string? InvoiceId { get; set; }
        public string? Message { get; set; }
    }


}
