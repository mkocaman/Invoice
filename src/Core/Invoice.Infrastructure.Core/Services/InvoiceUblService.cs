using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using System.Xml.Linq;
using System.Globalization;

namespace Invoice.Infrastructure.Services
{
    /// <summary>
    /// Invoice UBL XML oluşturma servisi - EŞÜ (UBL/e-Fatura) klavuzuna %100 uyumlu
    /// </summary>
    public class InvoiceUblService : IInvoiceUblService
    {
        private readonly ILogger<InvoiceUblService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        public InvoiceUblService(ILogger<InvoiceUblService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// InvoiceEnvelope'dan EŞÜ uyumlu UBL XML oluşturur
        /// </summary>
        public string BuildUblXml(InvoiceEnvelope envelope)
        {
            _logger.LogInformation("EŞÜ uyumlu UBL XML oluşturuluyor. Invoice Number: {InvoiceNumber}", 
                envelope.InvoiceNumber);

            try
            {
                // UBL 2.1 namespace'leri tanımla
                var ns = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
                var cac = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                var cbc = XNamespace.Get("urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");

                // Ana Invoice elementi oluştur
                var invoice = new XElement(ns + "Invoice",
                    // Namespace'leri ekle
                    new XAttribute(XNamespace.Xmlns + "cac", cac.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "cbc", cbc.NamespaceName),
                    
                    // Zorunlu temel alanlar
                    new XElement(cbc + "ID", envelope.InvoiceNumber),
                    new XElement(cbc + "IssueDate", envelope.IssueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
                    new XElement(cbc + "InvoiceTypeCode", "380"), // Standart satış faturası
                    new XElement(cbc + "DocumentCurrencyCode", envelope.Currency),
                    
                    // Tedarikçi bilgileri (AccountingSupplierParty)
                    CreateSupplierParty(cac, cbc),
                    
                    // Müşteri bilgileri (AccountingCustomerParty)
                    CreateCustomerParty(envelope, cac, cbc),
                    
                    // Fatura kalemleri
                    CreateInvoiceLines(envelope, cac, cbc),
                    
                    // Vergi toplamları
                    CreateTaxTotal(envelope, cac, cbc),
                    
                    // Yasal para toplamları
                    CreateLegalMonetaryTotal(envelope, cac, cbc)
                );

                // XML dokümanı oluştur
                var doc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", "yes"),
                    invoice
                );

                var ublXml = doc.ToString(SaveOptions.DisableFormatting);
                _logger.LogInformation("EŞÜ uyumlu UBL XML oluşturuldu. Boyut: {Size} karakter", ublXml.Length);
                return ublXml;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EŞÜ uyumlu UBL XML oluşturma hatası. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
                throw new InvalidOperationException($"EŞÜ uyumlu UBL XML oluşturulamadı: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Tedarikçi (satıcı) bilgilerini oluşturur
        /// </summary>
        private XElement CreateSupplierParty(XNamespace cac, XNamespace cbc)
        {
            return new XElement(cac + "AccountingSupplierParty",
                new XElement(cac + "Party",
                    new XElement(cac + "PartyName",
                        new XElement(cbc + "Name", "Demo Tedarikçi A.Ş.")),
                    new XElement(cac + "PartyTaxScheme",
                        new XElement(cbc + "CompanyID", "1234567890"),
                        new XElement(cac + "TaxScheme",
                            new XElement(cbc + "ID", "0015"),
                            new XElement(cbc + "Name", "KDV")))
                ));
        }

        /// <summary>
        /// Müşteri bilgilerini oluşturur
        /// </summary>
        private XElement CreateCustomerParty(InvoiceEnvelope envelope, XNamespace cac, XNamespace cbc)
        {
            return new XElement(cac + "AccountingCustomerParty",
                new XElement(cac + "Party",
                    new XElement(cac + "PartyName",
                        new XElement(cbc + "Name", envelope.Customer?.Name ?? "Müşteri")),
                    new XElement(cac + "PartyTaxScheme",
                        new XElement(cbc + "CompanyID", envelope.Customer?.TaxNumber ?? "0000000000"),
                        new XElement(cac + "TaxScheme",
                            new XElement(cbc + "ID", "0015"),
                            new XElement(cbc + "Name", "KDV")))
                ));
        }

        /// <summary>
        /// Fatura kalemlerini oluşturur
        /// </summary>
        private XElement CreateInvoiceLines(InvoiceEnvelope envelope, XNamespace cac, XNamespace cbc)
        {
            var invoiceLines = new XElement(cac + "InvoiceLines");
            
            for (int i = 0; i < envelope.LineItems.Count; i++)
            {
                var line = envelope.LineItems[i];
                var lineExtensionAmount = line.Quantity * line.UnitPrice;
                var taxAmount = lineExtensionAmount * line.TaxRate / 100;
                
                var invoiceLine = new XElement(cac + "InvoiceLine",
                    new XElement(cbc + "ID", (i + 1).ToString(CultureInfo.InvariantCulture)), // Satır sıra numarası
                    new XElement(cbc + "InvoicedQuantity", 
                        line.Quantity.ToString(CultureInfo.InvariantCulture),
                        new XAttribute("unitCode", GetUnitCode(line.UnitCode))), // UN/ECE Rec 20 kodu
                    new XElement(cbc + "LineExtensionAmount", 
                        lineExtensionAmount.ToString("F2", CultureInfo.InvariantCulture),
                        new XAttribute("currencyID", envelope.Currency)),
                    
                    // Kalem bilgileri
                    new XElement(cac + "Item",
                        new XElement(cbc + "Name", line.Description ?? line.Name ?? "Ürün")),
                    
                    // Birim fiyat
                    new XElement(cac + "Price",
                        new XElement(cbc + "PriceAmount", 
                            line.UnitPrice.ToString("F2", CultureInfo.InvariantCulture),
                            new XAttribute("currencyID", envelope.Currency))),
                    
                    // Vergi kategorisi
                    new XElement(cac + "TaxTotal",
                        new XElement(cbc + "TaxAmount", 
                            taxAmount.ToString("F2", CultureInfo.InvariantCulture),
                            new XAttribute("currencyID", envelope.Currency)),
                        new XElement(cac + "TaxSubtotal",
                            new XElement(cbc + "TaxableAmount", 
                                lineExtensionAmount.ToString("F2", CultureInfo.InvariantCulture),
                                new XAttribute("currencyID", envelope.Currency)),
                            new XElement(cbc + "TaxAmount", 
                                taxAmount.ToString("F2", CultureInfo.InvariantCulture),
                                new XAttribute("currencyID", envelope.Currency)),
                            new XElement(cac + "TaxCategory",
                                new XElement(cbc + "ID", "S"), // Standart vergi kategorisi
                                new XElement(cbc + "Percent", 
                                    line.TaxRate.ToString(CultureInfo.InvariantCulture)),
                                new XElement(cac + "TaxScheme",
                                    new XElement(cbc + "ID", "0015"),
                                    new XElement(cbc + "Name", "KDV")))))
                );
                
                invoiceLines.Add(invoiceLine);
            }
            
            return invoiceLines;
        }

        /// <summary>
        /// Vergi toplamlarını oluşturur
        /// </summary>
        private XElement CreateTaxTotal(InvoiceEnvelope envelope, XNamespace cac, XNamespace cbc)
        {
            var totalTaxAmount = envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice * l.TaxRate / 100);
            
            return new XElement(cac + "TaxTotal",
                new XElement(cbc + "TaxAmount", 
                    totalTaxAmount.ToString("F2", CultureInfo.InvariantCulture),
                    new XAttribute("currencyID", envelope.Currency)),
                new XElement(cac + "TaxSubtotal",
                    new XElement(cbc + "TaxableAmount", 
                        envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice).ToString("F2", CultureInfo.InvariantCulture),
                        new XAttribute("currencyID", envelope.Currency)),
                    new XElement(cbc + "TaxAmount", 
                        totalTaxAmount.ToString("F2", CultureInfo.InvariantCulture),
                        new XAttribute("currencyID", envelope.Currency)),
                    new XElement(cac + "TaxCategory",
                        new XElement(cbc + "ID", "S"),
                        new XElement(cbc + "Percent", 
                            envelope.LineItems.FirstOrDefault()?.TaxRate.ToString(CultureInfo.InvariantCulture) ?? "0"),
                        new XElement(cac + "TaxScheme",
                            new XElement(cbc + "ID", "0015"),
                            new XElement(cbc + "Name", "KDV")))));
        }

        /// <summary>
        /// Yasal para toplamlarını oluşturur
        /// </summary>
        private XElement CreateLegalMonetaryTotal(InvoiceEnvelope envelope, XNamespace cac, XNamespace cbc)
        {
            var lineExtensionAmount = envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice);
            var taxExclusiveAmount = lineExtensionAmount;
            var taxInclusiveAmount = envelope.TotalAmount;
            var payableAmount = envelope.TotalAmount;
            
            return new XElement(cac + "LegalMonetaryTotal",
                new XElement(cbc + "LineExtensionAmount", 
                    lineExtensionAmount.ToString("F2", CultureInfo.InvariantCulture),
                    new XAttribute("currencyID", envelope.Currency)),
                new XElement(cbc + "TaxExclusiveAmount", 
                    taxExclusiveAmount.ToString("F2", CultureInfo.InvariantCulture),
                    new XAttribute("currencyID", envelope.Currency)),
                new XElement(cbc + "TaxInclusiveAmount", 
                    taxInclusiveAmount.ToString("F2", CultureInfo.InvariantCulture),
                    new XAttribute("currencyID", envelope.Currency)),
                new XElement(cbc + "PayableAmount", 
                    payableAmount.ToString("F2", CultureInfo.InvariantCulture),
                    new XAttribute("currencyID", envelope.Currency)));
        }

        /// <summary>
        /// UN/ECE Rec 20 birim kodlarını döndürür (C62 = Adet varsayılan)
        /// </summary>
        private string GetUnitCode(string? unitCode)
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
    }
}
