using System.Xml;
using System.Xml.Schema;
using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// UBL XML validasyon servisi
/// </summary>
public class UblValidationService : IUblValidationService
{
    private readonly ILogger<UblValidationService> _logger;
    private readonly IUblSchemaResolver _schemaResolver;

    /// <summary>
    /// Constructor
    /// </summary>
    public UblValidationService(ILogger<UblValidationService> logger, IUblSchemaResolver schemaResolver)
    {
        _logger = logger;
        _schemaResolver = schemaResolver;
    }

    /// <summary>
    /// UBL XML'i şema ile doğrular
    /// </summary>
    public async Task<ValidationResult> ValidateUblAsync(string ublXml, string schemaVersion = "2.1")
    {
        _logger.LogInformation("UBL XML validasyonu başlatılıyor. Schema: {SchemaVersion}", schemaVersion);

        var result = new ValidationResult
        {
            IsValid = true,
            Errors = new List<string>(),
            Warnings = new List<string>()
        };

        try
        {
            // XML dokümanını yükle
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ublXml);

            // Şema dosyasını yükle
            var schemaPath = _schemaResolver.GetFullSchemaPath($"invoice-{schemaVersion}");
            
            if (!File.Exists(schemaPath))
            {
                result.IsValid = false;
                result.Errors.Add($"UBL şema dosyası bulunamadı: {schemaPath}");
                _logger.LogError("UBL şema dosyası bulunamadı: {SchemaPath}", schemaPath);
                return result;
            }

            // XML şema validasyonu yap
            var validationErrors = new List<string>();
            xmlDoc.Schemas.Add(null, schemaPath);
            xmlDoc.Validate((sender, e) =>
            {
                switch (e.Severity)
                {
                    case XmlSeverityType.Error:
                        validationErrors.Add($"Hata: {e.Message}");
                        break;
                    case XmlSeverityType.Warning:
                        result.Warnings.Add($"Uyarı: {e.Message}");
                        break;
                }
            });

            // Validasyon hatalarını sonuca ekle
            if (validationErrors.Any())
            {
                result.IsValid = false;
                result.Errors.AddRange(validationErrors);
                _logger.LogWarning("UBL XML validasyon hataları bulundu. Hata sayısı: {ErrorCount}", validationErrors.Count);
            }

            // İş kuralları validasyonu
            await ValidateBusinessRulesAsync(xmlDoc, result);

            _logger.LogInformation("UBL XML validasyonu tamamlandı. Geçerli: {IsValid}, Hata: {ErrorCount}, Uyarı: {WarningCount}", 
                result.IsValid, result.Errors.Count, result.Warnings.Count);

        }
        catch (XmlException ex)
        {
            result.IsValid = false;
            result.Errors.Add($"XML format hatası: {ex.Message}");
            _logger.LogError(ex, "UBL XML format hatası");
        }
        catch (Exception ex)
        {
            result.IsValid = false;
            result.Errors.Add($"Validasyon hatası: {ex.Message}");
            _logger.LogError(ex, "UBL XML validasyon hatası");
        }

        return result;
    }

    /// <summary>
    /// İş kuralları validasyonu
    /// </summary>
    private async Task ValidateBusinessRulesAsync(XmlDocument xmlDoc, ValidationResult result)
    {
        _logger.LogDebug("İş kuralları validasyonu başlatılıyor");

        try
        {
            var nsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            nsManager.AddNamespace("ubl", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

            // Zorunlu alanlar kontrolü
            await ValidateRequiredFieldsAsync(xmlDoc, nsManager, result);

            // Tarih kontrolü
            await ValidateDatesAsync(xmlDoc, nsManager, result);

            // Tutar kontrolü
            await ValidateAmountsAsync(xmlDoc, nsManager, result);

            // Vergi kontrolü
            await ValidateTaxesAsync(xmlDoc, nsManager, result);

            _logger.LogDebug("İş kuralları validasyonu tamamlandı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş kuralları validasyonu sırasında hata");
            result.Errors.Add($"İş kuralları validasyon hatası: {ex.Message}");
        }
    }

    /// <summary>
    /// Zorunlu alanlar kontrolü
    /// </summary>
    private async Task ValidateRequiredFieldsAsync(XmlDocument xmlDoc, XmlNamespaceManager nsManager, ValidationResult result)
    {
        var requiredFields = new[]
        {
            "//ubl:Invoice/ubl:ID",
            "//ubl:Invoice/ubl:IssueDate",
            "//ubl:Invoice/ubl:InvoiceTypeCode",
            "//ubl:Invoice/ubl:DocumentCurrencyCode",
            "//ubl:Invoice/ubl:AccountingSupplierParty",
            "//ubl:Invoice/ubl:AccountingCustomerParty"
        };

        foreach (var field in requiredFields)
        {
            var node = xmlDoc.SelectSingleNode(field, nsManager);
            if (node == null || string.IsNullOrWhiteSpace(node.InnerText))
            {
                result.Errors.Add($"Zorunlu alan eksik: {field}");
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Tarih kontrolü
    /// </summary>
    private async Task ValidateDatesAsync(XmlDocument xmlDoc, XmlNamespaceManager nsManager, ValidationResult result)
    {
        var issueDateNode = xmlDoc.SelectSingleNode("//ubl:Invoice/ubl:IssueDate", nsManager);
        if (issueDateNode != null && DateTime.TryParse(issueDateNode.InnerText, out var issueDate))
        {
            if (issueDate > DateTime.Today)
            {
                result.Warnings.Add("Fatura tarihi gelecek bir tarih olamaz");
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Tutar kontrolü
    /// </summary>
    private async Task ValidateAmountsAsync(XmlDocument xmlDoc, XmlNamespaceManager nsManager, ValidationResult result)
    {
        var lineExtensionAmountNode = xmlDoc.SelectSingleNode("//ubl:Invoice/ubl:LegalMonetaryTotal/ubl:LineExtensionAmount", nsManager);
        if (lineExtensionAmountNode != null && decimal.TryParse(lineExtensionAmountNode.InnerText, out var lineAmount))
        {
            if (lineAmount <= 0)
            {
                result.Errors.Add("Satır tutarı sıfırdan büyük olmalıdır");
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Vergi kontrolü
    /// </summary>
    private async Task ValidateTaxesAsync(XmlDocument xmlDoc, XmlNamespaceManager nsManager, ValidationResult result)
    {
        var taxAmountNode = xmlDoc.SelectSingleNode("//ubl:Invoice/ubl:LegalMonetaryTotal/ubl:TaxExclusiveAmount", nsManager);
        if (taxAmountNode != null && decimal.TryParse(taxAmountNode.InnerText, out var taxAmount))
        {
            if (taxAmount < 0)
            {
                result.Errors.Add("Vergi tutarı negatif olamaz");
            }
        }

        await Task.CompletedTask;
    }
}
