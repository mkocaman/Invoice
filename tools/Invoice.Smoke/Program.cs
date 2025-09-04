// Türkçe: Minimal smoke — UBL benzeri örnek üretip çıkar
using System.Xml.Linq;

class Program
{
    static int Main(string[] args)
    {
        // [SIMULATION] Basit UBL-vari çıktı
        var xml = new XDocument(
            new XElement("Invoice",
                new XElement("ID", "INV-SMOKE-001"),
                new XElement("IssueDate", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                new XElement("DocumentCurrencyCode", "TRY"),
                new XElement("LegalMonetaryTotal",
                    new XElement("PayableAmount", "100.00")
                )
            )
        );

        var path = Path.Combine(AppContext.BaseDirectory, "invoice-smoke.xml");
        xml.Save(path);
        Console.WriteLine($"[SMOKE] Üretildi: {path}");
        return 0;
    }
}