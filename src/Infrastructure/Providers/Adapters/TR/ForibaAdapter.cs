// Türkçe: FORIBA e-Fatura adapter (sandbox örnek). Gerçek uçlar için Foriba dokümantasyonuna göre tamamlayın.
using System.Net.Http.Headers;
using System.Text;
using Invoice.Application.Providers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Infrastructure.Options;

namespace Infrastructure.Providers.Adapters.TR
{
    public sealed class ForibaAdapter : IProviderAdapter
    {
        private readonly HttpClient _http;
        private readonly ProviderSecretOptions _secrets;
        private readonly ILogger<ForibaAdapter> _logger;

        public string CountryCode => "TR";
        public string Key => "FORIBA";

        public ForibaAdapter(Infrastructure.Providers.Http.IForibaClient cli, IOptions<ProviderSecretOptions> secrets, ILogger<ForibaAdapter> logger)
        {
            _http = cli.Client;
            _secrets = secrets.Value;
            _logger = logger;
        }

        public bool Supports(string capability) => capability.Contains("eInvoice", StringComparison.OrdinalIgnoreCase);

        public async Task<bool> HealthAsync(CancellationToken ct)
        {
            // [SIMULATION] Türkçe: Basit GET sağlığı; gerçek endpoint ile değiştirin.
            var resp = await _http.GetAsync("/health", ct);
            return resp.IsSuccessStatusCode;
        }

        public async Task<(bool ok, string? providerRef, string? rawResponse)> SendAsync(string invoiceId, string? ublOrJsonPayload, CancellationToken ct)
        {
            // Türkçe: Basit basic-auth; gerçek token akışını sağlayıcı dokümantasyonuna göre uygulayın.
            var user = _secrets.Foriba.Username;
            var pass = _secrets.Foriba.Password;
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{pass}"));
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            // [SIMULATION] Türkçe: Demo POST; gerçek endpoint/headers/body sağlayıcı kılavuzuna göre ayarlanmalı.
            var content = new StringContent(ublOrJsonPayload ?? "", Encoding.UTF8, "application/xml");
            var resp = await _http.PostAsync("/api/invoice/send", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            var ok = resp.IsSuccessStatusCode;

            _logger.LogInformation("FORIBA send invoiceId={InvoiceId} status={StatusCode}", invoiceId, (int)resp.StatusCode);
            return (ok, ok ? $"FORIBA-{invoiceId}" : null, body);
        }
    }
}
