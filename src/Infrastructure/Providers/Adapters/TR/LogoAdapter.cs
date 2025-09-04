// Türkçe: LOGO e-Fatura adapter (sandbox örnek). Gerçek uçlar için Logo API dokümantasyonuna göre tamamlayın.
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Invoice.Application.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Infrastructure.Options;

namespace Infrastructure.Providers.Adapters.TR
{
    public sealed class LogoAdapter : IProviderAdapter
    {
        private readonly HttpClient _http;
        private readonly ProviderSecretOptions _secrets;
        private readonly ILogger<LogoAdapter> _logger;

        public string CountryCode => "TR";
        public string Key => "LOGO";

        public LogoAdapter(Infrastructure.Providers.Http.ILogoClient cli, IOptions<ProviderSecretOptions> secrets, ILogger<LogoAdapter> logger)
        {
            _http = cli.Client;
            _secrets = secrets.Value;
            _logger = logger;
        }

        public bool Supports(string capability) => capability.Contains("eInvoice", StringComparison.OrdinalIgnoreCase);

        public async Task<bool> HealthAsync(CancellationToken ct)
        {
            var resp = await _http.GetAsync("/health", ct);
            return resp.IsSuccessStatusCode;
        }

        private async Task<string> GetTokenAsync(CancellationToken ct)
        {
            // [SIMULATION] Türkçe: client_credentials örneği — gerçek token endpoint ile değiştirin.
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _secrets.Logo.ClientId,
                ["client_secret"] = _secrets.Logo.ClientSecret,
            };
            var resp = await _http.PostAsync("/oauth2/token", new FormUrlEncodedContent(form), ct);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync(ct);
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("access_token").GetString()!;
        }

        public async Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(string invoiceId, string? ublOrJsonPayload, CancellationToken ct)
        {
            var token = await GetTokenAsync(ct);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(ublOrJsonPayload ?? "", Encoding.UTF8, "application/xml");
            var resp = await _http.PostAsync("/api/invoice/send", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            var ok = resp.IsSuccessStatusCode;

            _logger.LogInformation("LOGO send invoiceId={InvoiceId} status={StatusCode}", invoiceId, (int)resp.StatusCode);
            return (ok, ok ? $"LOGO-{invoiceId}" : null, body);
        }
    }
}
