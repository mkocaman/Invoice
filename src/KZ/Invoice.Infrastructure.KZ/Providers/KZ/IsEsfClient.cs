using System.Xml;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;

namespace Invoice.Infrastructure.Providers.KZ;

public interface IIsEsfClient
{
    Task<string> LoginAsync(string user, string pass, CancellationToken ct);
    Task<string> SendInvoiceXmlAsync(XDocument xml, string token, CancellationToken ct);
}

public sealed class IsEsfClient : IIsEsfClient
{
    private readonly IConfiguration _cfg;
    private readonly bool _sim;
    public IsEsfClient(IConfiguration cfg) { _cfg = cfg; _sim = _cfg.GetValue<bool>("Sandbox:Simulation"); }

    public Task<string> LoginAsync(string user, string pass, CancellationToken ct)
    {
        if (_sim)
        {
            var token = $"SIMULASYON_KZ_TOKEN_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            File.WriteAllText(Path.Combine("output","KZ_SANDBOX_TOKEN.json"), $"{{\"token\":\"{token}\",\"simulasyon\":true}}");
            return Task.FromResult(token);
        }

        // TODO: Generated proxy ile gerçek login çağrısı (WSDL metotlarına göre).
        throw new NotImplementedException("IS ESF real login TBD");
    }

    public Task<string> SendInvoiceXmlAsync(XDocument xml, string token, CancellationToken ct)
    {
        if (_sim)
        {
            var name = $"SIMULASYON_KZ_native_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
            xml.Save(Path.Combine("output", name));
            return Task.FromResult("SIM-SENT");
        }

        // TODO: token ile SOAP çağrısını yap (proxy üzerinden).
        throw new NotImplementedException("IS ESF real send TBD");
    }
}
