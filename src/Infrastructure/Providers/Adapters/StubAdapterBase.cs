// Türkçe Açıklama:
// Diğer 9 entegratör için basit simülasyon tabanlı adapter.
// İleride gerçek implementasyonla değiştirilebilir.

using Infrastructure.Providers.Config;
using Infrastructure.Providers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public abstract class StubAdapterBase : IInvoiceProvider
{
    protected readonly ProviderOptions Opts;
    protected readonly ILogger Logger;
    public abstract string Name { get; }

    protected ProviderConfig Cfg => Opts.Providers[Name];

    protected StubAdapterBase(IOptions<ProviderOptions> opts, ILogger logger)
    {
        Opts = opts.Value;
        Logger = logger;
    }

    public Task<bool> AuthenticateAsync(CancellationToken ct = default)
    {
        // Türkçe: Stub her zaman OK
        Logger.LogInformation("[SIMULATION][{Name}] Authenticate OK", Name);
        return Task.FromResult(true);
    }

    public Task<ProviderSendResult> SendAsync(string invoiceId, string? ublXml, string? jsonPayload, CancellationToken ct = default)
    {
        var req = jsonPayload ?? ublXml ?? "<xml>stub</xml>";
        var res = $"{{\"status\":\"OK\",\"provider\":\"{Name}\",\"externalId\":\"{Name}-TEST-0001\"}}";
        Logger.LogInformation("[SIMULATION][{Name}] Send OK invoice={InvoiceId}", Name, invoiceId);
        return Task.FromResult(new ProviderSendResult(true, $"{Name}-TEST-0001", req, res, null, null));
    }
}
