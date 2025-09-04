// Türkçe: Sağlayıcı HTTP istemcileri (typed) — timeout, baseurl ve auth akışları burada ayarlanır.
using Microsoft.Extensions.Options;
using Infrastructure.Options;

namespace Infrastructure.Providers.Http
{
    public interface IForibaClient
    {
        HttpClient Client { get; }
    }

    public interface ILogoClient
    {
        HttpClient Client { get; }
    }

    public sealed class ForibaClient : IForibaClient
    {
        public HttpClient Client { get; }
        public ForibaClient(HttpClient http, IOptions<ProviderSecretOptions> secrets)
        {
            // Türkçe: Foriba base adres + timeout
            http.BaseAddress = new Uri(secrets.Value.Foriba.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(secrets.Value.Foriba.TimeoutSeconds);
            Client = http;
        }
    }

    public sealed class LogoClient : ILogoClient
    {
        public HttpClient Client { get; }
        public LogoClient(HttpClient http, IOptions<ProviderSecretOptions> secrets)
        {
            http.BaseAddress = new Uri(secrets.Value.Logo.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(secrets.Value.Logo.TimeoutSeconds);
            Client = http;
        }
    }
}
