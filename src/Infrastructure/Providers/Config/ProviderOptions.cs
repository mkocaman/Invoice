// Türkçe Açıklama: Entegratör konfigürasyonları (appsettings binding)

namespace Infrastructure.Providers.Config;

public sealed class ProviderOptions
{
    // Türkçe: Hangi sağlayıcılar etkin? (örn. ["FORIBA","LOGO",...])
    public string[] Enabled { get; set; } = Array.Empty<string>();
    public required Dictionary<string, ProviderConfig> Providers { get; set; }
}

public sealed class ProviderConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public bool Simulation { get; set; } = true; // Türkçe: Stub'lar için varsayılan simülasyon
}
