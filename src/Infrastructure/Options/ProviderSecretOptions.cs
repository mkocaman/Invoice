// Türkçe: Sağlayıcı bazlı gizli bilgileri bağlamak için model (User-Secrets veya ortam değişkenleri).
namespace Infrastructure.Options
{
    public sealed class ProviderSecretOptions
    {
        // Türkçe: TR sağlayıcıları
        public ForibaOptions Foriba { get; set; } = new();
        public LogoOptions Logo { get; set; } = new();

        // Türkçe: Örnek diğer sağlayıcı anahtarları — 10+ sayısını korumak için iskelet bırakıyoruz
        public string? KolayBiApiKey { get; set; }
        public string? MikroApiKey { get; set; }
        public string? ParaşütApiKey { get; set; }
        public string? NebimClientId { get; set; }
        public string? NetsisToken { get; set; }
        public string? LucaToken { get; set; }
        public string? ZirveToken { get; set; }
        public string? EtaToken { get; set; }

        // Türkçe: Foriba
        public sealed class ForibaOptions
        {
            public string BaseUrl { get; set; } = "https://efaturatest.foriba.com.tr";
            public string Username { get; set; } = ""; // user-secrets
            public string Password { get; set; } = ""; // user-secrets
            public int TimeoutSeconds { get; set; } = 30;
        }

        // Türkçe: LOGO
        public sealed class LogoOptions
        {
            public string BaseUrl { get; set; } = "https://api.logo.com.tr/sandbox";
            public string ClientId { get; set; } = "";     // user-secrets
            public string ClientSecret { get; set; } = ""; // user-secrets
            public int TimeoutSeconds { get; set; } = 30;
        }
    }
}
