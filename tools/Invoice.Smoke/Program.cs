#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

static class UblNs {
    public static readonly XNamespace inv = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
    public static readonly XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
    public static readonly XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
}

static class UblSmoke
{
    public static XDocument BuildMinimalInvoice(
        string invoiceNumber,
        string currencyCode,
        string supplierName,
        string customerName,
        decimal qty,
        decimal unitPrice,
        decimal taxRatePercent,
        string unitCode = "C62")
    {
        var issueDate = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        decimal lineNet = unitPrice * qty;
        decimal taxAmt  = Math.Round(lineNet * (taxRatePercent/100m), 2);
        decimal total   = lineNet + taxAmt;

        var doc = new XDocument(
            new XComment("[SIMULASYON] – bu dosya test amaçlı üretilmiştir."),
            new XElement(UblNs.inv + "Invoice",
                new XAttribute(XNamespace.Xmlns + "cac", UblNs.cac.NamespaceName),
                new XAttribute(XNamespace.Xmlns + "cbc", UblNs.cbc.NamespaceName),

                new XElement(UblNs.cbc + "ID", invoiceNumber),
                new XElement(UblNs.cbc + "IssueDate", issueDate),
                new XElement(UblNs.cbc + "InvoiceTypeCode", "380"),
                new XElement(UblNs.cbc + "DocumentCurrencyCode", currencyCode),

                new XElement(UblNs.cac + "AccountingSupplierParty",
                    new XElement(UblNs.cac + "Party",
                        new XElement(UblNs.cbc + "Name", supplierName)
                    )
                ),
                new XElement(UblNs.cac + "AccountingCustomerParty",
                    new XElement(UblNs.cac + "Party",
                        new XElement(UblNs.cbc + "Name", customerName)
                    )
                ),

                // En az bir satır
                new XElement(UblNs.cac + "InvoiceLine",
                    new XElement(UblNs.cbc + "ID", "1"),
                    new XElement(UblNs.cbc + "InvoicedQuantity",
                        new XAttribute("unitCode", unitCode),
                        qty.ToString("F2", CultureInfo.InvariantCulture)
                    ),
                    new XElement(UblNs.cbc + "LineExtensionAmount",
                        lineNet.ToString("F2", CultureInfo.InvariantCulture)
                    ),
                    new XElement(UblNs.cac + "Item",
                        new XElement(UblNs.cbc + "Name", "SIM ITEM")
                    ),
                    new XElement(UblNs.cac + "Price",
                        new XElement(UblNs.cbc + "PriceAmount",
                            unitPrice.ToString("F2", CultureInfo.InvariantCulture)
                        )
                    ),
                    new XElement(UblNs.cac + "ClassifiedTaxCategory",
                        new XElement(UblNs.cbc + "ID", "S"),
                        new XElement(UblNs.cbc + "Percent", taxRatePercent.ToString("F2", CultureInfo.InvariantCulture)),
                        new XElement(UblNs.cac + "TaxScheme",
                            new XElement(UblNs.cbc + "ID", "VAT")
                        )
                    )
                ),

                new XElement(UblNs.cac + "TaxTotal",
                    new XElement(UblNs.cbc + "TaxAmount",
                        taxAmt.ToString("F2", CultureInfo.InvariantCulture)
                    )
                ),

                new XElement(UblNs.cac + "LegalMonetaryTotal",
                    new XElement(UblNs.cbc + "LineExtensionAmount",
                        lineNet.ToString("F2", CultureInfo.InvariantCulture)
                    ),
                    new XElement(UblNs.cbc + "TaxExclusiveAmount",
                        lineNet.ToString("F2", CultureInfo.InvariantCulture)
                    ),
                    new XElement(UblNs.cbc + "TaxInclusiveAmount",
                        total.ToString("F2", CultureInfo.InvariantCulture)
                    ),
                    new XElement(UblNs.cbc + "PayableAmount",
                        total.ToString("F2", CultureInfo.InvariantCulture)
                    )
                )
            )
        );
        return doc;
    }
}

static class Fs
{
    public static void EnsureDir(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}

class Program
{
    static int Main(string[] args)
    {
        try
        {
            Fs.EnsureDir("output");

            // TR – iki örnek (FORIBA, LOGO)
            var tr1 = UblSmoke.BuildMinimalInvoice("SIM-TR-0001", "TRY", "SIM SUPPLIER TR", "SIM CUSTOMER TR", 2m, 50m, 20m, "C62");
            tr1.Save(Path.Combine("output","SIMULASYON_TR_FORIBA.xml"));

            var tr2 = UblSmoke.BuildMinimalInvoice("SIM-TR-0002", "TRY", "SIM SUPPLIER TR", "SIM CUSTOMER TR", 1m, 18m, 10m, "C62");
            tr2.Save(Path.Combine("output","SIMULASYON_TR_LOGO.xml"));

            // UZ – placeholder (network/token yoksa yine de geçerli UBL)
            var uz = UblSmoke.BuildMinimalInvoice("SIM-UZ-0001", "UZS", "SIM SUPPLIER UZ", "SIM CUSTOMER UZ", 3m, 12m, 12m, "C62");
            uz.Save(Path.Combine("output","SIMULASYON_UZ_Didox.xml"));

            // KZ – placeholder
            var kz = UblSmoke.BuildMinimalInvoice("SIM-KZ-0001", "KZT", "SIM SUPPLIER KZ", "SIM CUSTOMER KZ", 5m, 7m, 12m, "C62");
            kz.Save(Path.Combine("output","SIMULASYON_KZ_IsEsf.xml"));

            Console.WriteLine("✅ SIMULASYON UBL dosyaları üretildi: output/SIMULASYON_*.xml");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("❌ Hata: " + ex);
            return 1;
        }
    }
}
