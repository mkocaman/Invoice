// Türkçe: Sağlayıcıya gönderim sözleşmesi (adapter). Somut sağlayıcılar bu interface'i uygular.
namespace Invoice.Application.Providers
{
    public interface IInvoiceSender
    {
        // Türkçe: Gerçek gönderim; başarılıysa sağlayıcı referansı/id vb. döndürebilir.
        Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(
            string countryCode, string providerKey, string invoiceId, string? payload, CancellationToken ct);
    }
}
