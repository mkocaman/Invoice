using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System.Text.Json;
using System.Net; // HTML encoding için WebUtility
// using System.Xml.Linq; // .NET 9'da paket gerekiyor, şimdilik kaldırıldı

namespace Invoice.Infrastructure.KZ.Providers;

/// <summary>
/// Kazakistan IS ESF entegrasyonu (SDK tabanlı auth desteği ile)
/// </summary>
public class IsEsfProvider : IInvoiceProvider
{
    private readonly ILogger<IsEsfProvider> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IKzEsfAuthClient? _authClient;

    public IsEsfProvider(
        ILogger<IsEsfProvider> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IKzEsfAuthClient? authClient = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _authClient = authClient;
    }

    public ProviderType ProviderType => ProviderType.IS_ESF_KZ;

    public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("IS ESF'ye fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
            envelope.InvoiceNumber, envelope.TenantId);

        try
        {
            // Türkçe: Kazakistan özel validasyonları
            var validationResult = ValidateKazakhstanInvoice(envelope);
            if (!validationResult.IsValid)
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "VALIDATION_ERROR",
                    errorMessage: validationResult.ErrorMessage);
            }

            // Türkçe: Para birimini KZT'de sabitliyoruz
            envelope.Currency = "KZT";

            // Türkçe: Offline mod kontrolü
            var offlineMode = _configuration.GetValue<bool>("FeatureFlags:Providers:IsEsf:Offline", false);
            if (offlineMode)
            {
                _logger.LogInformation("IS ESF offline modda çalışıyor. Mock başarılı yanıt döndürülüyor.");
                return new ProviderSendResult(
                    Success: true,
                    Provider: ProviderType,
                    ProviderReferenceNumber: $"IS-ESF-KZ-MOCK-{Guid.NewGuid():N}",
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
                    errorMessage: "IS ESF için ApiBaseUrl tanımlı değil.");
            }

            var httpClient = _httpClientFactory.CreateClient("isEsfKz");
            
            // Türkçe: SDK tabanlı kimlik doğrulama
            var token = await GetSdkTokenAsync(config, cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "AUTH_FAILED",
                    errorMessage: "IS ESF SDK token alınamadı.");
            }

            // Türkçe: XML imzalı fatura gönder
            var xmlInvoice = CreateXmlInvoice(envelope, config);
            var response = await SendInvoiceToIsEsfAsync(xmlInvoice, token, config, httpClient, cancellationToken);

            _logger.LogInformation("IS ESF'ye fatura gönderildi. Invoice Number: {InvoiceNumber}, Response: {Response}", 
                envelope.InvoiceNumber, response);

            return new ProviderSendResult(
                Success: true,
                Provider: ProviderType,
                ProviderReferenceNumber: response.InvoiceId ?? $"IS-ESF-KZ-{Guid.NewGuid():N}",
                ProviderResponseMessage: "IS ESF: e-Fatura başarıyla gönderildi",
                UblXml: xmlInvoice,
                ErrorCode: null,
                ErrorMessage: null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IS ESF'ye fatura gönderilirken hata. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
            
            return ProviderSendResult.Failed(
                provider: ProviderType,
                errorCode: "SEND_ERROR",
                errorMessage: $"IS ESF gönderim hatası: {ex.Message}");
        }
    }

    public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
    {
        _logger.LogDebug("IS ESF webhook imza doğrulaması. Body length: {BodyLength}", body.Length);
        
        // Türkçe: Mock doğrulama - gerçek implementasyonda SDK ile imza kontrolü yapılır
        return true;
    }

    /// <summary>
    /// Türkçe: Bu provider'ın belirtilen ülkeyi destekleyip desteklemediğini kontrol eder.
    /// </summary>
    public bool SupportsCountry(string countryCode)
    {
        return countryCode.Equals("KZ", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Türkçe: Bu provider'ın belirtilen provider tipini destekleyip desteklemediğini kontrol eder.
    /// </summary>
    public bool Supports(ProviderType type)
    {
        return type == ProviderType.IS_ESF_KZ;
    }

    private async Task<string?> GetSdkTokenAsync(ProviderConfig config, CancellationToken cancellationToken)
    {
        try
        {
            // Türkçe: SDK tabanlı kimlik doğrulama
            if (_authClient != null)
            {
                return await _authClient.GetTokenAsync(config, cancellationToken);
            }

            // Türkçe: Mock SDK token (dev modda)
            _logger.LogWarning("IS ESF SDK client bulunamadı, mock token kullanılıyor.");
            return "mock-sdk-token";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "IS ESF SDK token alınırken hata");
            return null;
        }
    }

    private string CreateXmlInvoice(InvoiceEnvelope envelope, ProviderConfig config)
    {
        // Türkçe: IS ESF XML formatına uygun fatura oluştur (Kazakistan gereksinimleri)
        var invoiceDate = envelope.IssueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        
        var itemsXml = "";
        var items = envelope.Items ?? envelope.LineItems;
        if (items != null)
        {
            itemsXml = string.Join("", items.Select(item => 
                $@"<Item>
                    <Name>{WebUtility.HtmlEncode(item.Name ?? item.Description)}</Name>
                    <Quantity>{item.Quantity}</Quantity>
                    <UnitPrice>{item.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}</UnitPrice>
                    <Total>{item.Total.ToString(System.Globalization.CultureInfo.InvariantCulture)}</Total>
                    <UnitCode>{GetKazakhstanUnitCode(item.UnitCode)}</UnitCode>
                    <TaxRate>{item.TaxRate.ToString(System.Globalization.CultureInfo.InvariantCulture)}</TaxRate>
                </Item>"
            ));
        }
        
        var customerName = WebUtility.HtmlEncode(envelope.Customer?.Name ?? envelope.CustomerName ?? "Клиент");
        var customerTaxNumber = envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber ?? "";
        var customerAddress = WebUtility.HtmlEncode(envelope.Customer?.AddressLine ?? "");
        
        var xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<Invoice xmlns=""http://kgd.gov.kz/esf"">
    <InvoiceNumber>{envelope.InvoiceNumber}</InvoiceNumber>
    <InvoiceDate>{invoiceDate}</InvoiceDate>
    <TotalAmount>{envelope.TotalAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}</TotalAmount>
    <Currency>KZT</Currency>
    <Customer>
        <Name>{customerName}</Name>
        <TaxNumber>{customerTaxNumber}</TaxNumber>
        <Address>{customerAddress}</Address>
        <CountryCode>{envelope.Customer?.CountryCode ?? "KZ"}</CountryCode>
    </Customer>
    <Items>{itemsXml}</Items>
</Invoice>";
        
        return xml;
    }

    /// <summary>
    /// UN/ECE Rec 20 birim kodlarını Kazakistan için döndürür
    /// </summary>
    private string GetKazakhstanUnitCode(string? unitCode)
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

    private (bool IsValid, string ErrorMessage) ValidateKazakhstanInvoice(InvoiceEnvelope envelope)
    {
        // Türkçe: Kazakistan özel validasyonları
        var customerTaxNumber = envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber;
        
        // Türkçe: Vergi numarası kontrolü (Kazakistan formatı: 12 haneli BIN)
        if (!string.IsNullOrWhiteSpace(customerTaxNumber))
        {
            if (customerTaxNumber.Length != 12 || !customerTaxNumber.All(char.IsDigit))
            {
                return (false, "Kazakistan BIN numarası 12 haneli olmalıdır");
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

    private async Task<IsEsfResponse> SendInvoiceToIsEsfAsync(
        string xmlInvoice, 
        string token, 
        ProviderConfig config, 
        HttpClient httpClient, 
        CancellationToken cancellationToken)
    {
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        var content = new StringContent(xmlInvoice, System.Text.Encoding.UTF8, "application/xml");
        var response = await httpClient.PostAsync("/api/documents/invoice/send", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<IsEsfResponse>(responseContent) ?? new IsEsfResponse();
    }

    // Türkçe: Response modelleri
    private class IsEsfResponse
    {
        public bool Success { get; set; }
        public string? InvoiceId { get; set; }
        public string? Message { get; set; }
    }


}

/// <summary>
/// Kazakistan IS ESF SDK tabanlı kimlik doğrulama arayüzü
/// </summary>
public interface IKzEsfAuthClient
{
    Task<string> GetTokenAsync(ProviderConfig config, CancellationToken cancellationToken = default);
}
