// Türkçe: Gerçek sağlayıcı adapter sözleşmesi (ülke/provide'a özgü entegrasyon).
namespace Invoice.Application.Providers
{
    public interface IProviderAdapter
    {
        string CountryCode { get; }  // "TR", "UZ", "KZ"
        string Key { get; }          // "FORIBA", "LOGO" vb.
        bool Supports(string capability); // "eInvoice", "eArchive" vb.

        // Türkçe: Sağlık kontrolü
        Task<bool> HealthAsync(CancellationToken ct);

        // Türkçe: Sağlayıcıya fatura gönderimi (provider send pipeline bunu çağırır)
        Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(
            string invoiceId, string? ublOrJsonPayload, CancellationToken ct);
    }
}
