// Türkçe: IInvoiceSender, seçilen IProviderAdapter üzerinden gerçek gönderim yapar.
using Invoice.Application.Providers;
using Invoice.Application.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Providers.Send
{
    public sealed class SimulatedInvoiceSender : IInvoiceSender
    {
        private readonly IServiceProvider _sp;
        public SimulatedInvoiceSender(IServiceProvider sp) => _sp = sp;

        public async Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(string countryCode, string providerKey, string invoiceId, string? payload, CancellationToken ct)
        {
            // Türkçe: Kayıtlı IProviderAdapter'ı çöz ve gönderimi ona delege et.
            var adapters = _sp.GetServices<IProviderAdapter>();
            var adapter = adapters.FirstOrDefault(a =>
                a.CountryCode.Equals(countryCode, StringComparison.OrdinalIgnoreCase) &&
                a.Key.Equals(providerKey, StringComparison.OrdinalIgnoreCase));

            if (adapter is null)
            {
                // [SIMULATION] Adapter yoksa önceki sahte davranış
                await Task.Delay(150, ct);
                var okSim = Random.Shared.NextDouble() > 0.2;
                var respSim = okSim ? $"OK-{providerKey}-{invoiceId}" : $"ERR-{providerKey}-{invoiceId}";
                return (okSim, okSim ? respSim : null, respSim);
            }

            return await adapter.SendAsync(invoiceId, payload, ct);
        }
    }
}
