// Türkçe: InvoiceEnvelope'dan minimum gerekli alanlarla UBL XML üretimini sağlayan yardımcı (basitleştirilmiş).
using System.Xml.Linq;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Core.Providers.Common
{
    public static class UblHelper
    {
        // Türkçe: Basitleştirilmiş UBL 2.1 oluşturur (entegratörlerin şemaları farklılık gösterebilir).
        public static string BuildMinimalUblXml(InvoiceEnvelope env)
        {
            // Not: Gerçek şemaya uygunluk için provider'a özel XSD doğrulama adımı ayrıca uygulanır.
            var inv = new XElement("Invoice",
                new XElement("ID", env.InvoiceNumber ?? Guid.NewGuid().ToString("N")),
                new XElement("IssueDate", env.IssueDate.ToString("yyyy-MM-dd")),
                new XElement("DocumentCurrencyCode", env.Currency ?? "TRY"),
                new XElement("AccountingSupplierParty",
                    new XElement("PartyName", "Supplier")),
                new XElement("AccountingCustomerParty",
                    new XElement("PartyName", env.Customer?.Name ?? "Customer")),
                new XElement("LegalMonetaryTotal",
                    new XElement("PayableAmount", env.TotalAmount))
            );

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), inv);
            return doc.ToString(SaveOptions.DisableFormatting);
        }
    }
}
