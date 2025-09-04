// Türkçe: Sağlayıcı yapılandırmalarını appsettings'ten bağlamak için model.
namespace Infrastructure.Providers
{
    public sealed class MultiProviderOptions
    {
        // Türkçe: Default ülke (örn: "TR")
        public string? DefaultCountry { get; set; }

        // Türkçe: Ülke -> Sağlayıcılar listesi
        public Dictionary<string, List<ProviderConfig>> Countries { get; set; } = new();

        // Türkçe: Sağlayıcı config'i
        public sealed class ProviderConfig
        {
            public string Key { get; set; } = "";        // Örn: "FORIBA", "LOGO"
            public string Name { get; set; } = "";       // Görsel ad
            public string BaseUrl { get; set; } = "";    // API kökü (sandbox/prod dışarıdan gelecektir)
            public bool Enabled { get; set; } = true;    // Açık/kapalı
            public int Priority { get; set; } = 100;     // Küçük = yüksek öncelik
            public int Weight { get; set; } = 1;         // Ağırlık (weighted round-robin)
            public int TimeoutSeconds { get; set; } = 30;// HTTP timeout
            public string? Capability { get; set; }      // Örn: "eInvoice,eArchive" gibi
        }
    }
}
