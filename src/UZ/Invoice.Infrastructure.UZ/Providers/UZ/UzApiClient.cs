using System.Globalization;
using System.Net.Http.Json;

namespace Invoice.Infrastructure.Providers.UZ;

public interface IUzApiClient
{
    Task<string> GetTokenAsync(CancellationToken ct);
    Task<UzInvoiceResponse> SendInvoiceAsync(UzInvoicePayload payload, string token, CancellationToken ct);
}

public sealed class UzApiClient : IUzApiClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    private readonly bool _sim;

    public UzApiClient(HttpClient http, IConfiguration cfg)
    {
        _http = http;
        _cfg = cfg;
        _sim = _cfg.GetValue<bool>("Sandbox:Simulation");
    }

    public async Task<string> GetTokenAsync(CancellationToken ct)
    {
        if (_sim)
        {
            var token = $"SIMULASYON_UZ_TOKEN_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
            var path = Path.Combine("output", "UZ_SANDBOX_TOKEN.json");
            await File.WriteAllTextAsync(path, $$"""{"token":"{{token}}","simulasyon":true}""", ct);
            return token;
        }

        var baseUrl = _cfg["Sandbox:UZ:BaseUrl"]!;
        var authPath = _cfg["Sandbox:UZ:AuthPath"]!;
        var signed = "PLACEHOLDER_SIGNED_DATA"; // E-IMZO entegrasyonu eklenecek
        var resp = await _http.PostAsJsonAsync(new Uri(new Uri(baseUrl), authPath), new UzAuthRequest(signed), ct);
        resp.EnsureSuccessStatusCode();
        var model = await resp.Content.ReadFromJsonAsync<UzAuthResponse>(cancellationToken: ct);
        return model!.AccessToken;
    }

    public async Task<UzInvoiceResponse> SendInvoiceAsync(UzInvoicePayload payload, string token, CancellationToken ct)
    {
        if (_sim)
        {
            var name = $"SIMULASYON_UZ_native_{payload.invoiceNumber}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            await File.WriteAllTextAsync(Path.Combine("output", name),
                System.Text.Json.JsonSerializer.Serialize(payload, new System.Text.Json.JsonSerializerOptions{WriteIndented=true}),
                ct);
            return new UzInvoiceResponse("SIM-ID", payload.invoiceNumber, "SIMULATED");
        }

        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var baseUrl = _cfg["Sandbox:UZ:BaseUrl"]!;
        var invPath = _cfg["Sandbox:UZ:InvoicePath"]!;
        var resp = await _http.PostAsJsonAsync(new Uri(new Uri(baseUrl), invPath), payload, ct);
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<UzInvoiceResponse>(cancellationToken: ct))!;
    }
}
