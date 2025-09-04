// Türkçe: Demo amaçlı sahte sender. Gerçekte sağlayıcı SDK/HTTP ile implement edin.
using Invoice.Application.Providers;

namespace Infrastructure.Providers.Send
{
    public sealed class SimulatedInvoiceSender : IInvoiceSender
    {
        public async Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(
            string countryCode, string providerKey, string invoiceId, string? payload, CancellationToken ct)
        {
            // [SIMULATION] Türkçe: Rastgele başarı/başarısız
            await Task.Delay(150, ct);
            var ok = Random.Shared.NextDouble() > 0.2; // %80 başarı
            var resp = ok ? $"OK-{providerKey}-{invoiceId}" : $"ERR-{providerKey}-{invoiceId}";
            return (ok, ok ? resp : null, resp);
        }
    }
}
