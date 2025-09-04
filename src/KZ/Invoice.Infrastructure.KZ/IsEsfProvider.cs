using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using System.Text.Json;
using System.Net; // HTML encoding için WebUtility
using System.Xml.Linq;
using Invoice.Infrastructure.Providers.KZ;

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
    private readonly IIsEsfClient _isEsfClient;

    public IsEsfProvider(
        ILogger<IsEsfProvider> logger,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IKzEsfAuthClient? authClient = null,
        IIsEsfClient? isEsfClient = null)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _authClient = authClient;
        _isEsfClient = isEsfClient ?? throw new ArgumentNullException(nameof(isEsfClient));
    }

    public ProviderType ProviderType => ProviderType.IS_ESF_KZ;
    
    public string Key => "is-esf-kz";
    
    public string CountryCode => "KZ";

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

            // Türkçe: Yeni IS ESF Client kullan
            var username = _configuration["Secrets:KZ:Username"] ?? "sandbox_user";
            var password = _configuration["Secrets:KZ:Password"] ?? "sandbox_pass";
            
            var token = await _isEsfClient.LoginAsync(username, password, cancellationToken);
            if (string.IsNullOrEmpty(token))
            {
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "AUTH_FAILED",
                    errorMessage: "IS ESF SDK token alınamadı.");
            }

            // Türkçe: XML imzalı fatura gönder (yeni client ile)
            var xmlInvoice = ToKzXml(envelope);
            var response = await _isEsfClient.SendInvoiceXmlAsync(xmlInvoice, token, cancellationToken);

            _logger.LogInformation("IS ESF'ye fatura gönderildi. Invoice Number: {InvoiceNumber}, Response: {Response}", 
                envelope.InvoiceNumber, response);

            return new ProviderSendResult(
                Success: true,
                Provider: ProviderType,
                ProviderReferenceNumber: response ?? $"IS-ESF-KZ-{Guid.NewGuid():N}",
                ProviderResponseMessage: "IS ESF: e-Fatura başarıyla gönderildi",
                UblXml: xmlInvoice.ToString(),
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

    /// <summary>
    /// InvoiceEnvelope'i KZ-native XML'e dönüştürür
    /// </summary>
    private XDocument ToKzXml(InvoiceEnvelope envelope)
    {
        var invoiceDate = envelope.IssueDate.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        
        var items = (envelope.Items ?? envelope.LineItems)?.Select(item => new
        {
            Name = WebUtility.HtmlEncode(item.Name ?? item.Description ?? "Unnamed"),
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            Total = item.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            UnitCode = GetKazakhstanUnitCode(item.UnitCode),
            TaxRate = item.TaxRate.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
        }).ToList();

        var net = items.Sum(item => decimal.Parse(item.Total.ToString())).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        var vat = items.Sum(item => decimal.Parse(item.Total.ToString()) * 0.12m).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        var gross = (decimal.Parse(net) + decimal.Parse(vat)).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

        var root = new XElement("Invoice");
        root.Add(new XElement("DocumentType", "KZ-LOCAL")); // [SIMULASYON]
        root.Add(new XElement("Number", envelope.InvoiceNumber));
        root.Add(new XElement("Date", invoiceDate));
        root.Add(new XElement("Currency", "KZT"));

        var supplier = new XElement("Supplier");
        supplier.Add(new XElement("BIN", envelope.Customer?.TaxNumber ?? "123456789012")); // Varsayılan BIN
        root.Add(supplier);

        var customer = new XElement("Customer");
        customer.Add(new XElement("BIN", envelope.Customer?.TaxNumber ?? envelope.CustomerTaxNumber ?? "210987654321"));
        root.Add(customer);

        var itemsElement = new XElement("Items");
        foreach (var item in items)
        {
            var itemElement = new XElement("Item");
            itemElement.Add(new XElement("Name", item.Name));
            itemElement.Add(new XElement("Quantity", item.Quantity));
            itemElement.Add(new XElement("UnitCode", item.UnitCode));
            itemElement.Add(new XElement("UnitPrice", item.UnitPrice));
            itemElement.Add(new XElement("LineExtensionAmount", item.Total));
            itemElement.Add(new XElement("VatPercent", item.TaxRate));
            itemElement.Add(new XElement("VatAmount", (decimal.Parse(item.Total.ToString()) * 0.12m).ToString("F2", System.Globalization.CultureInfo.InvariantCulture)));
            itemsElement.Add(itemElement);
        }
        root.Add(itemsElement);

        var totals = new XElement("Totals");
        totals.Add(new XElement("Net", net));
        totals.Add(new XElement("Vat", vat));
        totals.Add(new XElement("Gross", gross));
        root.Add(totals);

        var meta = new XElement("Meta");
        meta.Add(new XElement("Source", "InvoiceEnvelope"));
        meta.Add(new XElement("Simulasyon", "true"));
        root.Add(meta);

        return new XDocument(root);
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
