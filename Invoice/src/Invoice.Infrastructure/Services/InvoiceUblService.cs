using System.Xml;
using System.Xml.Linq;
using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Invoice UBL XML oluşturma servisi
/// </summary>
public class InvoiceUblService : IInvoiceUblService
{
    private readonly ILogger<InvoiceUblService> _logger;
    private readonly IUblValidationService _validationService;

    /// <summary>
    /// Constructor
    /// </summary>
    public InvoiceUblService(ILogger<InvoiceUblService> logger, IUblValidationService validationService)
    {
        _logger = logger;
        _validationService = validationService;
    }

    /// <summary>
    /// Invoice entity'sinden UBL XML oluşturur
    /// </summary>
    public async Task<string> CreateUblXmlAsync(Invoice.Domain.Entities.Invoice invoice, string schemaVersion = "2.1")
    {
        _logger.LogInformation("UBL XML oluşturuluyor. Invoice ID: {InvoiceId}, Schema: {SchemaVersion}", 
            invoice.Id, schemaVersion);

        try
        {
            // UBL namespace'leri
            var ublNamespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
            var cacNamespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            var cbcNamespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

            // Ana UBL XML dokümanını oluştur
            var ublDoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(XName.Get("Invoice", ublNamespace),
                    new XAttribute(XNamespace.Xmlns + "cac", cacNamespace),
                    new XAttribute(XNamespace.Xmlns + "cbc", cbcNamespace),
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute("xsi:schemaLocation", $"{ublNamespace} UBL-Invoice-{schemaVersion}.xsd")
                )
            );

            var invoiceElement = ublDoc.Root;

            // Temel bilgiler
            AddBasicInvoiceInfo(invoiceElement, invoice, cbcNamespace);
            
            // Tedarikçi bilgileri
            AddSupplierParty(invoiceElement, invoice, cacNamespace, cbcNamespace);
            
            // Müşteri bilgileri
            AddCustomerParty(invoiceElement, invoice, cacNamespace, cbcNamespace);
            
            // Fatura satırları
            AddInvoiceLines(invoiceElement, invoice, cacNamespace, cbcNamespace);
            
            // Toplam tutarlar
            AddLegalMonetaryTotal(invoiceElement, invoice, cacNamespace, cbcNamespace);
            
            // Vergi toplamları
            AddTaxTotal(invoiceElement, invoice, cacNamespace, cbcNamespace);

            var ublXml = ublDoc.ToString();
            
            _logger.LogInformation("UBL XML oluşturuldu. Boyut: {Size} karakter", ublXml.Length);

            // Validasyon yap
            var validationResult = await _validationService.ValidateUblAsync(ublXml, schemaVersion);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Oluşturulan UBL XML validasyon hatası var. Hata sayısı: {ErrorCount}", 
                    validationResult.Errors.Count);
            }

            return ublXml;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UBL XML oluşturma hatası. Invoice ID: {InvoiceId}", invoice.Id);
            throw new InvalidOperationException($"UBL XML oluşturulamadı: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Temel fatura bilgilerini ekler
    /// </summary>
    private void AddBasicInvoiceInfo(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cbcNamespace)
    {
        invoiceElement.Add(
            new XElement(XName.Get("ID", cbcNamespace), invoice.InvoiceNumber),
            new XElement(XName.Get("CopyIndicator", cbcNamespace), "false"),
            new XElement(XName.Get("UUID", cbcNamespace), invoice.Id.ToString()),
            new XElement(XName.Get("IssueDate", cbcNamespace), invoice.IssueDate.ToString("yyyy-MM-dd")),
            new XElement(XName.Get("IssueTime", cbcNamespace), invoice.IssueDate.ToString("HH:mm:ss")),
            new XElement(XName.Get("InvoiceTypeCode", cbcNamespace), "380"),
            new XElement(XName.Get("DocumentCurrencyCode", cbcNamespace), invoice.CurrencyCode ?? "TRY"),
            new XElement(XName.Get("LineCountNumeric", cbcNamespace), invoice.InvoiceLines?.Count ?? 0)
        );
    }

    /// <summary>
    /// Tedarikçi bilgilerini ekler
    /// </summary>
    private void AddSupplierParty(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cacNamespace, string cbcNamespace)
    {
        var supplierParty = new XElement(XName.Get("AccountingSupplierParty", cacNamespace));
        
        var party = new XElement(XName.Get("Party", cacNamespace));
        
        // Tedarikçi kimlik bilgileri
        var partyIdentification = new XElement(XName.Get("PartyIdentification", cacNamespace));
        partyIdentification.Add(new XElement(XName.Get("ID", cbcNamespace), 
            new XAttribute("schemeID", "VKN"), 
            invoice.Supplier?.TaxNumber ?? ""));
        party.Add(partyIdentification);

        // Tedarikçi adı
        var partyName = new XElement(XName.Get("PartyName", cacNamespace));
        partyName.Add(new XElement(XName.Get("Name", cbcNamespace), invoice.Supplier?.Name ?? ""));
        party.Add(partyName);

        // Tedarikçi adresi
        if (invoice.Supplier?.Address != null)
        {
            var postalAddress = new XElement(XName.Get("PostalAddress", cacNamespace));
            postalAddress.Add(
                new XElement(XName.Get("StreetName", cbcNamespace), invoice.Supplier.Address.Street ?? ""),
                new XElement(XName.Get("BuildingNumber", cbcNamespace), invoice.Supplier.Address.BuildingNumber ?? ""),
                new XElement(XName.Get("CitySubdivisionName", cbcNamespace), invoice.Supplier.Address.District ?? ""),
                new XElement(XName.Get("CityName", cbcNamespace), invoice.Supplier.Address.City ?? ""),
                new XElement(XName.Get("PostalZone", cbcNamespace), invoice.Supplier.Address.PostalCode ?? ""),
                new XElement(XName.Get("Country", cacNamespace),
                    new XElement(XName.Get("Name", cbcNamespace), "Türkiye"))
            );
            party.Add(postalAddress);
        }

        supplierParty.Add(party);
        invoiceElement.Add(supplierParty);
    }

    /// <summary>
    /// Müşteri bilgilerini ekler
    /// </summary>
    private void AddCustomerParty(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cacNamespace, string cbcNamespace)
    {
        var customerParty = new XElement(XName.Get("AccountingCustomerParty", cacNamespace));
        
        var party = new XElement(XName.Get("Party", cacNamespace));
        
        // Müşteri kimlik bilgileri
        var partyIdentification = new XElement(XName.Get("PartyIdentification", cacNamespace));
        partyIdentification.Add(new XElement(XName.Get("ID", cbcNamespace), 
            new XAttribute("schemeID", "VKN"), 
            invoice.Customer?.TaxNumber ?? ""));
        party.Add(partyIdentification);

        // Müşteri adı
        var partyName = new XElement(XName.Get("PartyName", cacNamespace));
        partyName.Add(new XElement(XName.Get("Name", cbcNamespace), invoice.Customer?.Name ?? ""));
        party.Add(partyName);

        // Müşteri adresi
        if (invoice.Customer?.Address != null)
        {
            var postalAddress = new XElement(XName.Get("PostalAddress", cacNamespace));
            postalAddress.Add(
                new XElement(XName.Get("StreetName", cbcNamespace), invoice.Customer.Address.Street ?? ""),
                new XElement(XName.Get("BuildingNumber", cbcNamespace), invoice.Customer.Address.BuildingNumber ?? ""),
                new XElement(XName.Get("CitySubdivisionName", cbcNamespace), invoice.Customer.Address.District ?? ""),
                new XElement(XName.Get("CityName", cbcNamespace), invoice.Customer.Address.City ?? ""),
                new XElement(XName.Get("PostalZone", cbcNamespace), invoice.Customer.Address.PostalCode ?? ""),
                new XElement(XName.Get("Country", cacNamespace),
                    new XElement(XName.Get("Name", cbcNamespace), "Türkiye"))
            );
            party.Add(postalAddress);
        }

        customerParty.Add(party);
        invoiceElement.Add(customerParty);
    }

    /// <summary>
    /// Fatura satırlarını ekler
    /// </summary>
    private void AddInvoiceLines(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cacNamespace, string cbcNamespace)
    {
        if (invoice.InvoiceLines == null || !invoice.InvoiceLines.Any())
        {
            _logger.LogWarning("Fatura satırı bulunamadı. Invoice ID: {InvoiceId}", invoice.Id);
            return;
        }

        foreach (var line in invoice.InvoiceLines)
        {
            var invoiceLine = new XElement(XName.Get("InvoiceLine", cacNamespace));
            
            invoiceLine.Add(
                new XElement(XName.Get("ID", cbcNamespace), line.LineNumber),
                new XElement(XName.Get("InvoicedQuantity", cbcNamespace), 
                    new XAttribute("unitCode", line.UnitCode ?? "C62"), 
                    line.Quantity),
                new XElement(XName.Get("LineExtensionAmount", cbcNamespace), 
                    new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                    line.LineTotal)
            );

            // Ürün bilgileri
            var item = new XElement(XName.Get("Item", cacNamespace));
            item.Add(new XElement(XName.Get("Name", cbcNamespace), line.ProductName ?? ""));
            invoiceLine.Add(item);

            // Fiyat bilgileri
            var price = new XElement(XName.Get("Price", cacNamespace));
            price.Add(new XElement(XName.Get("PriceAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                line.UnitPrice));
            invoiceLine.Add(price);

            invoiceElement.Add(invoiceLine);
        }
    }

    /// <summary>
    /// Toplam tutarları ekler
    /// </summary>
    private void AddLegalMonetaryTotal(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cacNamespace, string cbcNamespace)
    {
        var legalMonetaryTotal = new XElement(XName.Get("LegalMonetaryTotal", cacNamespace));
        
        legalMonetaryTotal.Add(
            new XElement(XName.Get("LineExtensionAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.SubTotal),
            new XElement(XName.Get("TaxExclusiveAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.SubTotal),
            new XElement(XName.Get("TaxInclusiveAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.TotalAmount),
            new XElement(XName.Get("AllowanceTotalAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.DiscountAmount ?? 0),
            new XElement(XName.Get("ChargeTotalAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.ChargeAmount ?? 0),
            new XElement(XName.Get("PayableAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.TotalAmount)
        );

        invoiceElement.Add(legalMonetaryTotal);
    }

    /// <summary>
    /// Vergi toplamlarını ekler
    /// </summary>
    private void AddTaxTotal(XElement invoiceElement, Invoice.Domain.Entities.Invoice invoice, string cacNamespace, string cbcNamespace)
    {
        var taxTotal = new XElement(XName.Get("TaxTotal", cacNamespace));
        
        taxTotal.Add(
            new XElement(XName.Get("TaxAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.TaxAmount)
        );

        // KDV detayları
        var taxSubtotal = new XElement(XName.Get("TaxSubtotal", cacNamespace));
        taxSubtotal.Add(
            new XElement(XName.Get("TaxableAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.SubTotal),
            new XElement(XName.Get("TaxAmount", cbcNamespace), 
                new XAttribute("currencyID", invoice.CurrencyCode ?? "TRY"), 
                invoice.TaxAmount),
            new XElement(XName.Get("Percent", cbcNamespace), invoice.TaxRate ?? 18),
            new XElement(XName.Get("TaxCategory", cacNamespace),
                new XElement(XName.Get("TaxScheme", cacNamespace),
                    new XElement(XName.Get("Name", cbcNamespace), "KDV"),
                    new XElement(XName.Get("TaxTypeCode", cbcNamespace), "0015")))
        );

        taxTotal.Add(taxSubtotal);
        invoiceElement.Add(taxTotal);
    }
}
