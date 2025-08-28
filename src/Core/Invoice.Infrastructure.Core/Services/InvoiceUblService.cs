using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Services
{
    /// <summary>
    /// Invoice UBL XML oluşturma servisi
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
        /// InvoiceEnvelope'dan UBL XML oluşturur
        /// </summary>
        public string BuildUblXml(InvoiceEnvelope envelope)
        {
            _logger.LogInformation("UBL XML oluşturuluyor. Invoice Number: {InvoiceNumber}", 
                envelope.InvoiceNumber);

            try
            {
                // Basit UBL 2.1 XML oluştur (TRY, 18% KDV örnekleri)
                // Şema validasyonu devre dışı - üretim öncesi açılır
                var ublXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<cac:Invoice xmlns:cac=""urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2""
             xmlns:cbc=""urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"">
    <cbc:ID>{envelope.InvoiceNumber}</cbc:ID>
    <cbc:IssueDate>{envelope.IssueDate:yyyy-MM-dd}</cbc:IssueDate>
    <cbc:DocumentCurrencyCode>TRY</cbc:DocumentCurrencyCode>
    <cac:AccountingSupplierParty>
        <cac:Party>
            <cac:PartyName>
                <cbc:Name>Demo Tedarikçi A.Ş.</cbc:Name>
            </cac:PartyName>
            <cac:PartyTaxScheme>
                <cbc:CompanyID>1234567890</cbc:CompanyID>
            </cac:PartyTaxScheme>
        </cac:Party>
    </cac:AccountingSupplierParty>
    <cac:AccountingCustomerParty>
        <cac:Party>
            <cac:PartyName>
                <cbc:Name>{envelope.Customer.Name}</cbc:Name>
            </cac:PartyName>
            <cac:PartyTaxScheme>
                <cbc:CompanyID>{envelope.Customer.TaxNumber}</cbc:CompanyID>
            </cac:PartyTaxScheme>
        </cac:Party>
    </cac:AccountingCustomerParty>
    <cac:InvoiceLines>
        {string.Join("", envelope.LineItems.Select((line, index) => $@"
        <cac:InvoiceLine>
            <cbc:ID>{index + 1}</cbc:ID>
            <cbc:InvoicedQuantity unitCode=""{line.UnitCode ?? "EA"}"">{line.Quantity}</cbc:InvoicedQuantity>
            <cbc:LineExtensionAmount currencyID=""TRY"">{line.Quantity * line.UnitPrice}</cbc:LineExtensionAmount>
            <cac:Item>
                <cbc:Description>{line.Description}</cbc:Description>
            </cac:Item>
            <cac:Price>
                <cbc:PriceAmount currencyID=""TRY"">{line.UnitPrice}</cbc:PriceAmount>
            </cac:Price>
            <cac:TaxTotal>
                <cbc:TaxAmount currencyID=""TRY"">{line.Quantity * line.UnitPrice * line.TaxRate / 100}</cbc:TaxAmount>
                <cac:TaxSubtotal>
                    <cbc:TaxableAmount currencyID=""TRY"">{line.Quantity * line.UnitPrice}</cbc:TaxableAmount>
                    <cbc:TaxAmount currencyID=""TRY"">{line.Quantity * line.UnitPrice * line.TaxRate / 100}</cbc:TaxAmount>
                    <cac:TaxCategory>
                        <cac:TaxScheme>
                            <cbc:ID>0015</cbc:ID>
                            <cbc:Name>KDV</cbc:Name>
                        </cac:TaxScheme>
                    </cac:TaxCategory>
                </cac:TaxSubtotal>
            </cac:TaxTotal>
        </cac:InvoiceLine>"))}
    </cac:InvoiceLines>
    <cac:LegalMonetaryTotal>
        <cbc:LineExtensionAmount currencyID=""TRY"">{envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice)}</cbc:LineExtensionAmount>
        <cbc:TaxExclusiveAmount currencyID=""TRY"">{envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice)}</cbc:TaxExclusiveAmount>
        <cbc:TaxInclusiveAmount currencyID=""TRY"">{envelope.LineItems.Sum(l => l.Quantity * l.UnitPrice * (1 + l.TaxRate / 100))}</cbc:TaxInclusiveAmount>
        <cbc:PayableAmount currencyID=""TRY"">{envelope.TotalAmount}</cbc:PayableAmount>
    </cac:LegalMonetaryTotal>
</cac:Invoice>";

                _logger.LogInformation("UBL XML oluşturuldu. Boyut: {Size} karakter", ublXml.Length);
                return ublXml;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UBL XML oluşturma hatası. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
                throw new InvalidOperationException($"UBL XML oluşturulamadı: {ex.Message}", ex);
            }
        }
    }
}
