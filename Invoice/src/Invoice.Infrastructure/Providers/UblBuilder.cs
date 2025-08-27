using Invoice.Application.Models;
using System.Xml;
using System.Xml.Linq;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// UBL XML üretimi - SelfSign modunda kullanılır
/// </summary>
public class UblBuilder
{
    private readonly ILogger<UblBuilder> _logger;
    private readonly bool _disableXsdValidation;

    /// <summary>
    /// UBL namespace'leri
    /// </summary>
    private static readonly XNamespace UblNamespace = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    private static readonly XNamespace CacNamespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
    private static readonly XNamespace CbcNamespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="disableXsdValidation">XSD doğrulamasını devre dışı bırak</param>
    public UblBuilder(ILogger<UblBuilder> logger, bool disableXsdValidation = false)
    {
        _logger = logger;
        _disableXsdValidation = disableXsdValidation;
    }

    /// <summary>
    /// InvoiceEnvelope'dan UBL XML üretir
    /// </summary>
    /// <param name="envelope">Fatura zarfı</param>
    /// <param name="supplierInfo">Tedarikçi bilgileri</param>
    /// <returns>UBL XML string</returns>
    public string BuildUblXml(InvoiceEnvelope envelope, SupplierInfo supplierInfo)
    {
        try
        {
            _logger.LogDebug("UBL XML üretimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

            var invoice = new XElement(UblNamespace + "Invoice",
                // Temel fatura bilgileri
                new XElement(CbcNamespace + "ID", envelope.InvoiceNumber),
                new XElement(CbcNamespace + "CopyIndicator", "false"),
                new XElement(CbcNamespace + "UUID", envelope.InvoiceId.ToString()),
                new XElement(CbcNamespace + "IssueDate", envelope.InvoiceDate.ToString("yyyy-MM-dd")),
                new XElement(CbcNamespace + "IssueTime", envelope.InvoiceDate.ToString("HH:mm:ss")),
                new XElement(CbcNamespace + "InvoiceTypeCode", "SATIS"),
                new XElement(CbcNamespace + "DocumentCurrencyCode", envelope.Currency),
                new XElement(CbcNamespace + "LineCountNumeric", envelope.LineItems.Count),

                // Tedarikçi bilgileri
                BuildPartyElement("AccountingSupplierParty", supplierInfo),

                // Müşteri bilgileri
                BuildPartyElement("AccountingCustomerParty", envelope.Customer),

                // Fatura satırları
                envelope.LineItems.Select(BuildInvoiceLine),

                // Toplam tutarlar
                BuildTaxTotal(envelope.TaxAmount),
                BuildLegalMonetaryTotal(envelope.SubTotal, envelope.TaxAmount, envelope.TotalAmount)
            );

            var document = new XDocument(new XDeclaration("1.0", "UTF-8", null), invoice);

            // XSD doğrulama (opsiyonel)
            if (!_disableXsdValidation)
            {
                ValidateUblXml(document);
            }

            var xmlString = document.ToString();
            _logger.LogDebug("UBL XML başarıyla üretildi. Boyut: {Size} karakter", xmlString.Length);

            return xmlString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UBL XML üretim hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            throw new InvalidOperationException("UBL XML üretim hatası", ex);
        }
    }

    /// <summary>
    /// Taraf bilgilerini UBL formatında oluşturur
    /// </summary>
    /// <param name="elementName">Element adı</param>
    /// <param name="party">Taraf bilgileri</param>
    /// <returns>UBL party element</returns>
    private XElement BuildPartyElement(string elementName, object party)
    {
        var partyElement = new XElement(CacNamespace + elementName,
            new XElement(CacNamespace + "Party",
                new XElement(CacNamespace + "PartyIdentification",
                    new XElement(CbcNamespace + "ID", GetTaxNumber(party))
                ),
                new XElement(CacNamespace + "PartyName",
                    new XElement(CbcNamespace + "Name", GetPartyName(party))
                ),
                new XElement(CacNamespace + "PostalAddress",
                    new XElement(CbcNamespace + "StreetName", GetAddress(party)),
                    new XElement(CbcNamespace + "CityName", "İSTANBUL"),
                    new XElement(CbcNamespace + "PostalZone", "34000"),
                    new XElement(CacNamespace + "Country",
                        new XElement(CbcNamespace + "Name", "Türkiye")
                    )
                ),
                new XElement(CacNamespace + "PartyTaxScheme",
                    new XElement(CacNamespace + "TaxScheme",
                        new XElement(CbcNamespace + "Name", GetTaxOffice(party))
                    )
                )
            )
        );

        return partyElement;
    }

    /// <summary>
    /// Fatura satırını UBL formatında oluşturur
    /// </summary>
    /// <param name="lineItem">Satır kalemi</param>
    /// <returns>UBL invoice line element</returns>
    private XElement BuildInvoiceLine(InvoiceLineItem lineItem)
    {
        return new XElement(CacNamespace + "InvoiceLine",
            new XElement(CbcNamespace + "ID", lineItem.LineId.ToString()),
            new XElement(CbcNamespace + "InvoicedQuantity", lineItem.Quantity.ToString("F2"), 
                new XAttribute("unitCode", lineItem.Unit)),
            new XElement(CbcNamespace + "LineExtensionAmount", lineItem.LineTotal.ToString("F2"),
                new XAttribute("currencyID", "TRY")),
            new XElement(CacNamespace + "Item",
                new XElement(CbcNamespace + "Name", lineItem.Description),
                new XElement(CbcNamespace + "Description", lineItem.Description)
            ),
            new XElement(CacNamespace + "Price",
                new XElement(CbcNamespace + "PriceAmount", lineItem.UnitPrice.ToString("F2"),
                    new XAttribute("currencyID", "TRY"))
            ),
            BuildTaxSubtotal(lineItem.TaxAmount, lineItem.TaxRate)
        );
    }

    /// <summary>
    /// KDV alt toplamını oluşturur
    /// </summary>
    /// <param name="taxAmount">KDV tutarı</param>
    /// <param name="taxRate">KDV oranı</param>
    /// <returns>UBL tax subtotal element</returns>
    private XElement BuildTaxSubtotal(decimal taxAmount, decimal taxRate)
    {
        return new XElement(CacNamespace + "TaxSubtotal",
            new XElement(CbcNamespace + "TaxableAmount", "0.00", new XAttribute("currencyID", "TRY")),
            new XElement(CbcNamespace + "TaxAmount", taxAmount.ToString("F2"), new XAttribute("currencyID", "TRY")),
            new XElement(CacNamespace + "TaxCategory",
                new XElement(CbcNamespace + "Percent", taxRate.ToString("F2")),
                new XElement(CacNamespace + "TaxScheme",
                    new XElement(CbcNamespace + "Name", "KDV")
                )
            )
        );
    }

    /// <summary>
    /// KDV toplamını oluşturur
    /// </summary>
    /// <param name="taxAmount">KDV tutarı</param>
    /// <returns>UBL tax total element</returns>
    private XElement BuildTaxTotal(decimal taxAmount)
    {
        return new XElement(CacNamespace + "TaxTotal",
            new XElement(CbcNamespace + "TaxAmount", taxAmount.ToString("F2"), new XAttribute("currencyID", "TRY"))
        );
    }

    /// <summary>
    /// Yasal para toplamını oluşturur
    /// </summary>
    /// <param name="subTotal">Ara toplam</param>
    /// <param name="taxAmount">KDV tutarı</param>
    /// <param name="totalAmount">Genel toplam</param>
    /// <returns>UBL legal monetary total element</returns>
    private XElement BuildLegalMonetaryTotal(decimal subTotal, decimal taxAmount, decimal totalAmount)
    {
        return new XElement(CacNamespace + "LegalMonetaryTotal",
            new XElement(CbcNamespace + "LineExtensionAmount", subTotal.ToString("F2"), new XAttribute("currencyID", "TRY")),
            new XElement(CbcNamespace + "TaxExclusiveAmount", subTotal.ToString("F2"), new XAttribute("currencyID", "TRY")),
            new XElement(CbcNamespace + "TaxInclusiveAmount", totalAmount.ToString("F2"), new XAttribute("currencyID", "TRY")),
            new XElement(CbcNamespace + "PayableAmount", totalAmount.ToString("F2"), new XAttribute("currencyID", "TRY"))
        );
    }

    /// <summary>
    /// UBL XML'i XSD ile doğrular
    /// </summary>
    /// <param name="document">XML document</param>
    private void ValidateUblXml(XDocument document)
    {
        // XSD doğrulama implementasyonu (şimdilik boş)
        _logger.LogDebug("XSD doğrulama atlandı (dev ortamında)");
    }

    // Helper metodlar
    private string GetTaxNumber(object party) => party switch
    {
        CustomerInfo customer => customer.TaxNumber ?? "11111111111",
        SupplierInfo supplier => supplier.TaxNumber,
        _ => "11111111111"
    };

    private string GetPartyName(object party) => party switch
    {
        CustomerInfo customer => customer.Name,
        SupplierInfo supplier => supplier.Name,
        _ => "Bilinmeyen"
    };

    private string GetAddress(object party) => party switch
    {
        CustomerInfo customer => customer.Address ?? "Adres bilgisi yok",
        SupplierInfo supplier => supplier.Address,
        _ => "Adres bilgisi yok"
    };

    private string GetTaxOffice(object party) => party switch
    {
        CustomerInfo customer => customer.TaxOffice ?? "Vergi Dairesi",
        SupplierInfo supplier => supplier.TaxOffice,
        _ => "Vergi Dairesi"
    };
}

/// <summary>
/// Tedarikçi bilgileri
/// </summary>
public class SupplierInfo
{
    /// <summary>
    /// Tedarikçi adı
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// VKN
    /// </summary>
    public string TaxNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Vergi dairesi
    /// </summary>
    public string TaxOffice { get; set; } = string.Empty;
    
    /// <summary>
    /// Adres
    /// </summary>
    public string Address { get; set; } = string.Empty;
}
