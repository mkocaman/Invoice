// Türkçe: InvoiceEnvelope'dan UBL 2.1 standartlarına uygun XML üretimini sağlayan yardımcı
using System.Xml.Linq;
using System.Globalization;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Core.Providers.Common
{
    public static class UblHelper
    {
        // Türkçe: UBL 2.1 namespace'leri
        private static class UblNs
        {
            public static readonly XNamespace inv = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
            public static readonly XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
            public static readonly XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        }

        // Türkçe: EŞÜ uyumlu UBL 2.1 oluşturur
        public static string BuildMinimalUblXml(InvoiceEnvelope env)
        {
            var doc = new XDocument(
                new XElement(UblNs.inv + "Invoice",
                    // Namespace'leri ekle
                    new XAttribute(XNamespace.Xmlns + "cac", UblNs.cac.NamespaceName),
                    new XAttribute(XNamespace.Xmlns + "cbc", UblNs.cbc.NamespaceName),

                    // Zorunlu temel alanlar
                    new XElement(UblNs.cbc + "ID", env.InvoiceNumber ?? "SIM-INV-0001"),
                    new XElement(UblNs.cbc + "IssueDate", env.IssueDate.ToString("yyyy-MM-dd")),
                    new XElement(UblNs.cbc + "InvoiceTypeCode", "380"), // Standart satış faturası
                    new XElement(UblNs.cbc + "DocumentCurrencyCode", env.Currency ?? "TRY"),

                    // Tedarikçi bilgileri (varsayılan)
                    new XElement(UblNs.cac + "AccountingSupplierParty",
                        new XElement(UblNs.cac + "Party",
                            new XElement(UblNs.cac + "PartyName",
                                new XElement(UblNs.cbc + "Name", "SIM-LEVER"))
                        )
                    ),

                    // Müşteri bilgileri
                    new XElement(UblNs.cac + "AccountingCustomerParty",
                        new XElement(UblNs.cac + "Party",
                            new XElement(UblNs.cac + "PartyName",
                                new XElement(UblNs.cbc + "Name", env.Customer?.Name ?? "SIM-CUSTOMER"))
                        )
                    ),

                    // Fatura kalemleri
                    CreateInvoiceLines(env),

                    // Vergi toplamı
                    CreateTaxTotal(env),

                    // Yasal para toplamları
                    CreateLegalMonetaryTotal(env)
                )
            );

            return doc.ToString(SaveOptions.DisableFormatting);
        }

        private static XElement CreateInvoiceLines(InvoiceEnvelope env)
        {
            var invoiceLines = new XElement(UblNs.cac + "InvoiceLines");
            
            if (env.LineItems?.Any() == true)
            {
                foreach (var (item, index) in env.LineItems.Select((item, index) => (item, index)))
                {
                    var invoiceLine = new XElement(UblNs.cac + "InvoiceLine",
                        new XElement(UblNs.cbc + "ID", (index + 1).ToString(CultureInfo.InvariantCulture)),
                        new XElement(UblNs.cbc + "InvoicedQuantity",
                            new XAttribute("unitCode", item.UnitCode ?? "C62"), // UN/ECE Rec 20: C62 = adet
                            item.Quantity.ToString("F2", CultureInfo.InvariantCulture)
                        ),
                        new XElement(UblNs.cbc + "LineExtensionAmount",
                            new XAttribute("currencyID", env.Currency ?? "TRY"),
                            item.Total.ToString("F2", CultureInfo.InvariantCulture)
                        ),
                        new XElement(UblNs.cac + "Item",
                            new XElement(UblNs.cbc + "Name", item.Name ?? item.Description ?? "SIM ITEM")
                        ),
                        new XElement(UblNs.cac + "Price",
                            new XElement(UblNs.cbc + "PriceAmount",
                                new XAttribute("currencyID", env.Currency ?? "TRY"),
                                item.UnitPrice.ToString("F2", CultureInfo.InvariantCulture)
                            )
                        )
                    );
                    invoiceLines.Add(invoiceLine);
                }
            }
            else
            {
                // Varsayılan satır
                var defaultLine = new XElement(UblNs.cac + "InvoiceLine",
                    new XElement(UblNs.cbc + "ID", "1"),
                    new XElement(UblNs.cbc + "InvoicedQuantity",
                        new XAttribute("unitCode", "C62"),
                        "1.00"
                    ),
                    new XElement(UblNs.cbc + "LineExtensionAmount",
                        new XAttribute("currencyID", env.Currency ?? "TRY"),
                        "100.00"
                    ),
                    new XElement(UblNs.cac + "Item",
                        new XElement(UblNs.cbc + "Name", "SIM ITEM")
                    ),
                    new XElement(UblNs.cac + "Price",
                        new XElement(UblNs.cbc + "PriceAmount",
                            new XAttribute("currencyID", env.Currency ?? "TRY"),
                            "100.00"
                        )
                    )
                );
                invoiceLines.Add(defaultLine);
            }

            return invoiceLines;
        }

        private static XElement CreateTaxTotal(InvoiceEnvelope env)
        {
            var taxAmount = 0m;
            
            if (env.LineItems?.Any() == true)
            {
                taxAmount = env.LineItems.Sum(x => x.Total * (x.TaxRate / 100m));
            }

            return new XElement(UblNs.cac + "TaxTotal",
                new XElement(UblNs.cbc + "TaxAmount",
                    new XAttribute("currencyID", env.Currency ?? "TRY"),
                    taxAmount.ToString("F2", CultureInfo.InvariantCulture)
                )
            );
        }

        private static XElement CreateLegalMonetaryTotal(InvoiceEnvelope env)
        {
            var lineExtensionAmount = env.LineItems?.Sum(x => x.Total) ?? 100m;
            var taxAmount = env.LineItems?.Sum(x => x.Total * (x.TaxRate / 100m)) ?? 18m; // Varsayılan %18 KDV
            var taxInclusiveAmount = lineExtensionAmount + taxAmount;

            return new XElement(UblNs.cac + "LegalMonetaryTotal",
                new XElement(UblNs.cbc + "LineExtensionAmount",
                    new XAttribute("currencyID", env.Currency ?? "TRY"),
                    lineExtensionAmount.ToString("F2", CultureInfo.InvariantCulture)
                ),
                new XElement(UblNs.cbc + "TaxExclusiveAmount",
                    new XAttribute("currencyID", env.Currency ?? "TRY"),
                    lineExtensionAmount.ToString("F2", CultureInfo.InvariantCulture)
                ),
                new XElement(UblNs.cbc + "TaxInclusiveAmount",
                    new XAttribute("currencyID", env.Currency ?? "TRY"),
                    taxInclusiveAmount.ToString("F2", CultureInfo.InvariantCulture)
                ),
                new XElement(UblNs.cbc + "PayableAmount",
                    new XAttribute("currencyID", env.Currency ?? "TRY"),
                    taxInclusiveAmount.ToString("F2", CultureInfo.InvariantCulture)
                )
            );
        }
    }
}
