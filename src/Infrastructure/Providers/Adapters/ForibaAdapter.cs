// Türkçe Açıklama:
// Foriba gerçek adapter. Auth → Send akışını HttpClient ile yapar.
// Burada endpoint/mapping iskeleti verilir, gerçek JSON/XML map'leri provider dökümanına göre doldurulmalı.

using System.Net.Http.Json;
using Infrastructure.Providers.Config;
using Infrastructure.Providers.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Providers.Adapters;

public sealed class ForibaAdapter : IInvoiceProvider
{
    private readonly HttpClient _http;
    private readonly ILogger<ForibaAdapter> _logger;
    private readonly ProviderOptions _opts;
    private ProviderConfig Cfg => _opts.Providers["FORIBA"];

    public string Name => "FORIBA";

    public ForibaAdapter(HttpClient http, IOptions<ProviderOptions> opts, ILogger<ForibaAdapter> logger)
    {
        _http = http;
        _opts = opts.Value;
        _logger = logger;

        // Türkçe: HttpClient temel ayarları
        _http.BaseAddress = new Uri(Cfg.BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(Cfg.TimeoutSeconds);
    }

    public async Task<bool> AuthenticateAsync(CancellationToken ct = default)
    {
        // [SIMULATION]: Config Simulation true ise sahte başarı dön
        if (Cfg.Simulation)
        {
            _logger.LogInformation("[SIMULATION][FORIBA] Authenticate OK");
            return true;
        }

        // TODO: Foriba auth çağrısı (örn. token endpoint)
        // var req = new { username = Cfg.Username, password = Cfg.Password };
        // var res = await _http.PostAsJsonAsync("/auth", req, ct);
        // if (!res.IsSuccessStatusCode) return false;
        // ... token'ı sakla (HttpClient auth header vs.)
        _logger.LogInformation("FORIBA Authenticate (stubbed real call)");
        return true;
    }

    public async Task<ProviderSendResult> SendAsync(string invoiceId, string? ublXml, string? jsonPayload, CancellationToken ct = default)
    {
        // [SIMULATION]: simülasyon ise sahte yanıt
        if (Cfg.Simulation)
        {
            var fakeReq = jsonPayload ?? ublXml ?? "<xml>stub</xml>";
            var fakeRes = "{\"status\":\"OK\",\"externalId\":\"F-TEST-0001\"}";
            return new ProviderSendResult(true, "F-TEST-0001", fakeReq, fakeRes, null, null);
        }

        // TODO: Foriba gerçek gönderim (UBL/XML veya JSON mapping)
        // var req = new { invoice = ublXml ?? jsonPayload };
        // using var res = await _http.PostAsJsonAsync("/invoices/send", req, ct);
        // var body = await res.Content.ReadAsStringAsync(ct);
        // return res.IsSuccessStatusCode
        //   ? new ProviderSendResult(true, ParseExternalNumber(body), JsonConvert.SerializeObject(req), body, null, null)
        //   : new ProviderSendResult(false, null, JsonConvert.SerializeObject(req), body, res.StatusCode.ToString(), "Send failed");

        _logger.LogInformation("FORIBA Send (stubbed real call)");
        return new ProviderSendResult(true, "F-REAL-PLACEHOLDER", jsonPayload ?? ublXml, "{\"ok\":true}", null, null);
    }
}
