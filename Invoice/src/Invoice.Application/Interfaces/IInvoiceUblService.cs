using InvoiceEntity = Invoice.Domain.Entities.Invoice;

namespace Invoice.Application.Interfaces;

/// <summary>
/// UBL XML üretimi için servis sözleşmesi
/// </summary>
public interface IInvoiceUblService
{
    /// <summary>
    /// Invoice entity'sinden UBL XML oluşturur
    /// </summary>
    /// <param name="invoice">Fatura entity'si</param>
    /// <param name="schemaVersion">UBL şema versiyonu</param>
    /// <returns>UBL XML string</returns>
    Task<string> CreateUblXmlAsync(InvoiceEntity invoice, string schemaVersion = "2.1");
}
