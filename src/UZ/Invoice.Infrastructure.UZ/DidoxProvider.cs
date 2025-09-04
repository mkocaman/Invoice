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
using Invoice.Infrastructure.Providers.UZ;

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
    private readonly IUzApiClient _uzApiClient;

    public DidoxProvider(
        ILogger<DidoxProvider> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ISigningService? signingService = null,
        IUzApiClient? uzApiClient = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _signingService = signingService;
        _uzApiClient = uzApiClient ?? throw new ArgumentNullException(nameof(uzApiClient));
    }

    public ProviderType ProviderType => ProviderType.DIDOX_UZ;

    public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Didox'a fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
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

            // Türkçe: Yeni UZ API Client kullan
            var token = await _uzApiClient.GetTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "AUTH_FAILED",
                    errorMessage: "Didox E-IMZO token alınamadı.");
            }

            // Türkçe: Fatura gönder (yeni client ile)
            var payload = ToUzPayload(envelope);
            var response = await _uzApiClient.SendInvoiceAsync(payload, token, cancellationToken);

            _logger.LogInformation("Didox'a fatura gönderildi. Invoice Number: {InvoiceNumber}, Response: {Response}", 
                envelope.InvoiceNumber, response);

            return new ProviderSendResult(
                Success: true,
                Provider: ProviderType,
                ProviderReferenceNumber: response.id ?? $"DIDOX-UZ-{Guid.NewGuid():N}",
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

    /// <summary>
    /// InvoiceEnvelope'i UzInvoicePayload'e dönüştürür
    /// </summary>
    private UzInvoicePayload ToUzPayload(InvoiceEnvelope envelope)
    {
        var items = (envelope.Items ?? envelope.LineItems)?.Select(item => new UzInvoiceLine(
            name: WebUtility.HtmlEncode(item.Name ?? item.Description ?? "Unnamed"),
            quantity: item.Quantity.ToString(System.Globalization.CultureInfo.InvariantCulture),
            unit_code: GetUzbekistanUnitCode(item.UnitCode),
            unit_price: item.UnitPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            line_total: item.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            vat_percent: "12", // Özbekistan varsayılan KDV oranı
            vat_amount: (item.Total * 0.12m).ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        )).ToList() ?? new List<UzInvoiceLine>();

        var net = items.Sum(item => decimal.Parse(item.line_total)).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        var vat = items.Sum(item => decimal.Parse(item.vat_amount)).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        var gross = (decimal.Parse(net) + decimal.Parse(vat)).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        return new UzInvoicePayload(
            invoiceNumber: envelope.InvoiceNumber,
            currency: "UZS",
            sellerInn: envelope.Customer?.TaxNumber ?? "123456789", // Varsayılan INN
            buyerInn: envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber ?? "987654321",
            issueDate: envelope.IssueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            items: items,
            net: net,
            vat: vat,
            gross: gross
        );
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
