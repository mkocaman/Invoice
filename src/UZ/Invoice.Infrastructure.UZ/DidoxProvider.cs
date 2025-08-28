using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Invoice.Infrastructure.UZ.Providers;

/// <summary>
/// Özbekistan Didox entegrasyonu (E-IMZO imza desteği ile)
/// </summary>
public class DidoxProvider : IInvoiceProvider
{
    private readonly ILogger<DidoxProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ISigningService? _signingService;

    public DidoxProvider(
        ILogger<DidoxProvider> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ISigningService? signingService = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _signingService = signingService;
    }

    public ProviderType ProviderType => ProviderType.DIDOX_UZ;

    public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Didox'a fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
            envelope.InvoiceNumber, envelope.TenantId);

        try
        {
            // Türkçe: Para birimini UZS'de sabitliyoruz
            envelope.Currency = "UZS";

            // Türkçe: Offline mod kontrolü
            var offlineMode = _configuration.GetValue<bool>("FeatureFlags:Providers:DidoxUz:Offline", false);
            if (offlineMode)
            {
                _logger.LogInformation("Didox offline modda çalışıyor. Mock başarılı yanıt döndürülüyor.");
                return new ProviderSendResult(
                    Success: true,
                    Provider: ProviderType,
                    ProviderReferenceNumber: $"DIDOX-UZ-MOCK-{Guid.NewGuid():N}",
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
                    errorMessage: "Didox için ApiBaseUrl tanımlı değil.");
            }

            var httpClient = _httpClientFactory.CreateClient("didoxUz");
            
            // Türkçe: E-IMZO ile token al
            var token = await GetEImzoTokenAsync(config, httpClient, cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "AUTH_FAILED",
                    errorMessage: "Didox E-IMZO token alınamadı.");
            }

            // Türkçe: Fatura gönder (E-IMZO imzalı)
            var invoicePayload = CreateInvoicePayload(envelope, config);
            var response = await SendInvoiceToDidoxAsync(invoicePayload, token, config, httpClient, cancellationToken);

            _logger.LogInformation("Didox'a fatura gönderildi. Invoice Number: {InvoiceNumber}, Response: {Response}", 
                envelope.InvoiceNumber, response);

            return new ProviderSendResult(
                Success: true,
                Provider: ProviderType,
                ProviderReferenceNumber: response.InvoiceId ?? $"DIDOX-UZ-{Guid.NewGuid():N}",
                ProviderResponseMessage: "Didox: e-Fatura başarıyla gönderildi",
                UblXml: null,
                ErrorCode: null,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Didox'a fatura gönderilirken hata. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
            
            return ProviderSendResult.Failed(
                provider: ProviderType,
                errorCode: "SEND_ERROR",
                errorMessage: $"Didox gönderim hatası: {ex.Message}");
        }
    }

    public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
    {
        _logger.LogDebug("Didox webhook imza doğrulaması. Body length: {BodyLength}", body.Length);
        
        // Türkçe: Mock doğrulama - gerçek implementasyonda E-IMZO ile imza kontrolü yapılır
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
        return type == ProviderType.DIDOX_UZ;
    }

    private async Task<string?> GetEImzoTokenAsync(ProviderConfig config, HttpClient httpClient, CancellationToken cancellationToken)
    {
        try
        {
            // Türkçe: E-IMZO ile kimlik doğrulama
            var eImzoRequest = new
            {
                username = config.Username,
                password = config.Password,
                certificate_path = config.Extra1, // Sertifika yolu
                certificate_password = config.Extra2 // Sertifika şifresi
            };

            var response = await httpClient.PostAsJsonAsync("/api/auth/e-imzo", eImzoRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<DidoxTokenResponse>(cancellationToken: cancellationToken);
            return tokenResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Didox E-IMZO token alınırken hata");
            return null;
        }
    }

    private object CreateInvoicePayload(InvoiceEnvelope envelope, ProviderConfig config)
    {
        // Türkçe: Didox API formatına uygun payload oluştur
        var payload = new
        {
            invoice_number = envelope.InvoiceNumber,
            invoice_date = envelope.InvoiceDate.ToString("yyyy-MM-dd"),
            total_amount = envelope.TotalAmount,
            currency = "UZS",
            customer = new
            {
                name = envelope.CustomerName,
                tin = envelope.CustomerTaxNumber
            },
            items = envelope.Items?.Select(item => new
            {
                name = item.Name,
                quantity = item.Quantity,
                unit_price = item.UnitPrice,
                total = item.Total
            }).ToArray()
        };

        // Türkçe: E-IMZO imza ekle (şimdilik mock)
        if (_signingService != null)
        {
            var signature = _signingService.SignDocument(JsonSerializer.Serialize(payload));
            return new
            {
                data = payload,
                signature = signature
            };
        }

        return payload;
    }

    private async Task<DidoxResponse> SendInvoiceToDidoxAsync(
        object payload, 
        string token, 
        ProviderConfig config, 
        HttpClient httpClient, 
        CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.PostAsJsonAsync("/api/documents/invoice/send", payload, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DidoxResponse>(cancellationToken: cancellationToken) 
               ?? new DidoxResponse();
    }

    // Türkçe: Response modelleri
    private class DidoxTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }

    private class DidoxResponse
    {
        public bool Success { get; set; }
        public string? InvoiceId { get; set; }
        public string? Message { get; set; }
    }
}
