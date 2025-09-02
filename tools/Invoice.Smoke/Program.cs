using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Infrastructure;
using Invoice.Infrastructure.TR;
using Invoice.Infrastructure.UZ;
using Invoice.Infrastructure.KZ;
using System.Globalization;
using System.Xml.Linq;

// Türkçe: EŞÜ uyumluluğu için smoke test - her provider için UBL XML çıktısı üretir
var services = new ServiceCollection();

// Türkçe: Configuration ekle
var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection()
    .Build();

// Türkçe: Configuration servisini ekle
services.AddSingleton<IConfiguration>(configuration);

// Türkçe: Logging ekle
services.AddLogging(builder => 
{
    builder.SetMinimumLevel(LogLevel.Information);
});

// Türkçe: Infrastructure servislerini ekle
services.AddInfrastructure("Host=localhost;Database=invoice_test");
services.AddInvoiceProviders();
services.AddInvoiceProvidersTR();
services.AddInvoiceProvidersUZ(configuration);
services.AddInvoiceProvidersKZ(configuration);

var sp = services.BuildServiceProvider();
var factory = sp.GetRequiredService<IInvoiceProviderFactory>();
var ublService = sp.GetRequiredService<IInvoiceUblService>();

// Türkçe: Test verisi oluştur
var testEnvelope = CreateTestInvoiceEnvelope();

Console.WriteLine("=== EŞÜ Uyumluluğu Smoke Test ===");
Console.WriteLine($"Test Fatura No: {testEnvelope.InvoiceNumber}");
Console.WriteLine($"Test Tarih: {testEnvelope.IssueDate:yyyy-MM-dd}");
Console.WriteLine($"Test Tutar: {testEnvelope.TotalAmount:F2} {testEnvelope.Currency}");
Console.WriteLine();

// Türkçe: TR Provider'larını test et
Console.WriteLine("== TR Destekli Provider'lar ==");
await TestProvider(factory, ublService, testEnvelope, "TR", "FORIBA");
await TestProvider(factory, ublService, testEnvelope, "TR", "LOGO");

// Türkçe: UZ Provider'larını test et
Console.WriteLine("\n== UZ Destekli Provider'lar ==");
await TestProvider(factory, ublService, testEnvelope, "UZ", "DIDOX_UZ");
await TestProvider(factory, ublService, testEnvelope, "UZ", "FAKTURA_UZ");

// Türkçe: KZ Provider'larını test et
Console.WriteLine("\n== KZ Destekli Provider'lar ==");
await TestProvider(factory, ublService, testEnvelope, "KZ", "IS_ESF_KZ");

Console.WriteLine("\n=== Test Tamamlandı ===");
Console.WriteLine("UBL XML dosyaları 'output' klasörüne yazıldı.");

/// <summary>
/// Türkçe: Test için örnek fatura zarfı oluşturur
/// </summary>
static InvoiceEnvelope CreateTestInvoiceEnvelope()
{
    return new InvoiceEnvelope
    {
        TenantId = "test-tenant",
        InvoiceNumber = $"TEST-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 20),
        IssueDate = DateTime.UtcNow.Date,
        Currency = "TRY", // Ülkeye göre değişecek
        TotalAmount = 118.00m,
        Customer = new CustomerInfo
        {
            Name = "Test Müşteri A.Ş.",
            TaxNumber = "1234567890",
            TaxOffice = "Test Vergi Dairesi",
            Email = "test@example.com",
            CountryCode = "TR",
            AddressLine = "Test Adres, Test Mahallesi, Test/İSTANBUL"
        },
        LineItems = new List<InvoiceLineItem>
        {
            new InvoiceLineItem
            {
                Description = "Test Ürün 1",
                Quantity = 2,
                UnitPrice = 50.00m,
                TaxRate = 18.0m,
                UnitCode = "C62" // UN/ECE Rec 20: Adet
            },
            new InvoiceLineItem
            {
                Description = "Test Hizmet 1",
                Quantity = 1,
                UnitPrice = 18.00m,
                TaxRate = 18.0m,
                UnitCode = "HUR" // UN/ECE Rec 20: Saat
            }
        }
    };
}

/// <summary>
/// Türkçe: Belirtilen provider'ı test eder ve UBL XML çıktısını dosyaya yazar
/// </summary>
static async Task TestProvider(
    IInvoiceProviderFactory factory, 
    IInvoiceUblService ublService, 
    InvoiceEnvelope envelope, 
    string countryCode, 
    string providerKey)
{
    try
    {
        Console.WriteLine($" • {countryCode}-{providerKey}");
        
        // Türkçe: Provider'ı çöz
        var provider = factory.Resolve(countryCode, providerKey);
        if (provider == null)
        {
            Console.WriteLine($"   ❌ Provider bulunamadı: {providerKey}");
            return;
        }

        // Türkçe: Ülkeye göre test verilerini ayarla
        var testEnvelope = CloneAndAdjustForCountry(envelope, countryCode);
        
        // Türkçe: Provider'ı test et
        var config = new ProviderConfig { ApiBaseUrl = "https://test.example.com" };
        var result = await provider.SendInvoiceAsync(testEnvelope, config);
        
        if (result.Success)
        {
            Console.WriteLine($"   ✅ Başarılı: {result.ProviderReferenceNumber}");
            
            // Türkçe: EŞÜ uyumlu UBL XML oluştur
            var ublXml = ublService.BuildUblXml(testEnvelope);
            
            // Türkçe: Dosyaya yaz
            var fileName = $"output/{countryCode}_{providerKey}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
            Directory.CreateDirectory("output");
            await File.WriteAllTextAsync(fileName, ublXml);
            
            Console.WriteLine($"   📄 UBL XML: {fileName}");
            
            // Türkçe: Zorunlu alanları kontrol et
            ValidateUblFields(ublXml, fileName);
        }
        else
        {
            Console.WriteLine($"   ❌ Hata: {result.ErrorMessage}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   ❌ Exception: {ex.Message}");
    }
}

/// <summary>
/// Türkçe: Ülkeye göre test verilerini ayarlar
/// </summary>
static InvoiceEnvelope CloneAndAdjustForCountry(InvoiceEnvelope original, string countryCode)
{
    var clone = new InvoiceEnvelope
    {
        TenantId = original.TenantId,
        InvoiceNumber = original.InvoiceNumber,
        IssueDate = original.IssueDate,
        TotalAmount = original.TotalAmount,
        Customer = new CustomerInfo
        {
            Name = original.Customer.Name,
            TaxNumber = original.Customer.TaxNumber,
            TaxOffice = original.Customer.TaxOffice,
            Email = original.Customer.Email,
            AddressLine = original.Customer.AddressLine
        },
        LineItems = original.LineItems.Select(item => new InvoiceLineItem
        {
            Description = item.Description,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            TaxRate = item.TaxRate,
            UnitCode = item.UnitCode
        }).ToList()
    };

    // Türkçe: Ülkeye göre ayarlamalar
    switch (countryCode.ToUpperInvariant())
    {
        case "UZ":
            clone.Currency = "UZS";
            clone.Customer.CountryCode = "UZ";
            clone.Customer.TaxNumber = "123456789"; // 9 haneli Özbekistan vergi no
            break;
        case "KZ":
            clone.Currency = "KZT";
            clone.Customer.CountryCode = "KZ";
            clone.Customer.TaxNumber = "123456789012"; // 12 haneli Kazakistan BIN
            break;
        case "TR":
        default:
            clone.Currency = "TRY";
            clone.Customer.CountryCode = "TR";
            clone.Customer.TaxNumber = "1234567890"; // 10 haneli Türkiye vergi no
            break;
    }

    return clone;
}

/// <summary>
/// Türkçe: UBL XML'de zorunlu alanların varlığını LINQ to XML ile kontrol eder
/// </summary>
static void ValidateUblFields(string ublXml, string fileName)
{
    try
    {
        // Türkçe: XML'i parse et
        var doc = XDocument.Parse(ublXml);
        
        // Türkçe: UBL namespace'leri tanımla
        var inv = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
        var cac = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
        var cbc = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
        
        var missingFields = new List<string>();
        
        // Türkçe: Temel fatura alanları
        if (doc.Root?.Element(cbc + "InvoiceTypeCode") == null)
            missingFields.Add("cbc:InvoiceTypeCode");
            
        if (doc.Root?.Element(cbc + "IssueDate") == null)
            missingFields.Add("cbc:IssueDate");
            
        if (doc.Root?.Element(cbc + "DocumentCurrencyCode") == null)
            missingFields.Add("cbc:DocumentCurrencyCode");
        
        // Türkçe: Taraflar
        if (doc.Root?.Element(cac + "AccountingSupplierParty") == null)
            missingFields.Add("cac:AccountingSupplierParty");
            
        if (doc.Root?.Element(cac + "AccountingCustomerParty") == null)
            missingFields.Add("cac:AccountingCustomerParty");
        
        // Türkçe: Vergi toplamı
        var taxTotal = doc.Root?.Element(cac + "TaxTotal");
        if (taxTotal?.Element(cbc + "TaxAmount") == null)
            missingFields.Add("cac:TaxTotal/cbc:TaxAmount");
        
        // Türkçe: Para toplamları
        var monetaryTotal = doc.Root?.Element(cac + "LegalMonetaryTotal");
        if (monetaryTotal != null)
        {
            if (monetaryTotal.Element(cbc + "LineExtensionAmount") == null)
                missingFields.Add("cac:LegalMonetaryTotal/cbc:LineExtensionAmount");
                
            if (monetaryTotal.Element(cbc + "TaxExclusiveAmount") == null)
                missingFields.Add("cac:LegalMonetaryTotal/cbc:TaxExclusiveAmount");
                
            if (monetaryTotal.Element(cbc + "TaxInclusiveAmount") == null)
                missingFields.Add("cac:LegalMonetaryTotal/cbc:TaxInclusiveAmount");
                
            if (monetaryTotal.Element(cbc + "PayableAmount") == null)
                missingFields.Add("cac:LegalMonetaryTotal/cbc:PayableAmount");
        }
        else
        {
            missingFields.Add("cac:LegalMonetaryTotal");
        }
        
        // Türkçe: Satır kalemleri
        var invoiceLines = doc.Root?.Elements(cac + "InvoiceLine").ToList();
        if (invoiceLines != null && invoiceLines.Any())
        {
            for (int i = 0; i < invoiceLines.Count; i++)
            {
                var line = invoiceLines[i];
                var lineNumber = i + 1;
                
                if (line.Element(cbc + "ID") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cbc:ID");
                    
                if (line.Element(cbc + "InvoicedQuantity")?.Attribute("unitCode") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cbc:InvoicedQuantity@unitCode");
                    
                if (line.Element(cbc + "LineExtensionAmount") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cbc:LineExtensionAmount");
                    
                if (line.Element(cac + "Item")?.Element(cbc + "Name") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cac:Item/cbc:Name");
                    
                if (line.Element(cac + "Price")?.Element(cbc + "PriceAmount") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cac:Price/cbc:PriceAmount");
                    
                if (line.Element(cac + "ClassifiedTaxCategory") == null)
                    missingFields.Add($"InvoiceLine[{lineNumber}]/cac:ClassifiedTaxCategory");
            }
        }
        else
        {
            missingFields.Add("cac:InvoiceLine");
        }
        
        if (missingFields.Any())
        {
            Console.WriteLine($"   ⚠️  Eksik alanlar ({missingFields.Count}):");
            foreach (var field in missingFields.Take(5)) // İlk 5'i göster
            {
                Console.WriteLine($"      - {field}");
            }
            if (missingFields.Count > 5)
            {
                Console.WriteLine($"      ... ve {missingFields.Count - 5} alan daha");
            }
        }
        else
        {
            Console.WriteLine($"   ✅ Tüm zorunlu alanlar mevcut");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   ❌ XML doğrulama hatası: {ex.Message}");
    }
}
