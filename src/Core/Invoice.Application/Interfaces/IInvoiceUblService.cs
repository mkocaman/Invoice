using Invoice.Application.Models;

namespace Invoice.Application.Interfaces;

/// <summary>
/// UBL XML üretimi için servis sözleşmesi
/// </summary>
public interface IInvoiceUblService
{
    /// <summary>
    /// InvoiceEnvelope'dan UBL XML oluşturur
    /// </summary>
    /// <param name="envelope">Fatura zarfı</param>
    /// <returns>UBL XML string</returns>
    string BuildUblXml(InvoiceEnvelope envelope);
}
