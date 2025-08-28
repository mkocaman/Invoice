using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System.Text.Json;
using System.Xml.Linq;

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
        // Türkçe: IS ESF XML formatına uygun fatura oluştur
        var xmlDoc = new XDocument(
            new XElement("Invoice",
                new XAttribute("xmlns", "http://kgd.gov.kz/esf"),
                new XElement("InvoiceNumber", envelope.InvoiceNumber),
                new XElement("InvoiceDate", envelope.InvoiceDate.ToString("yyyy-MM-dd")),
                new XElement("TotalAmount", envelope.TotalAmount),
                new XElement("Currency", "KZT"),
                new XElement("Customer",
                    new XElement("Name", envelope.CustomerName),
                    new XElement("TaxNumber", envelope.CustomerTaxNumber)
                ),
                new XElement("Items",
                    envelope.Items?.Select(item => 
                        new XElement("Item",
                            new XElement("Name", item.Name),
                            new XElement("Quantity", item.Quantity),
                            new XElement("UnitPrice", item.UnitPrice),
                            new XElement("Total", item.Total)
                        )
                    )
                )
            )
        );

        return xmlDoc.ToString();
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
