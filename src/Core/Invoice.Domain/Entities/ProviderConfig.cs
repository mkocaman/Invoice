using System;
using Invoice.Domain.Enums;

namespace Invoice.Domain.Entities
{
    // Türkçe: Entegratör konfigürasyonu (çok kiracılı yapı için TenantId ile unique)
    public class ProviderConfig : BaseEntity
    {
        public string TenantId { get; set; } = default!;
        public ProviderType ProviderType { get; set; }
        public string ProviderKey { get; set; } = default!;
        
        // Çok-ülkeli destek
        public string CountryCode { get; set; } = "TR"; // TR, UZ, KZ
        public string Currency { get; set; } = "TRY"; // TRY, UZS, KZT
        public string AuthType { get; set; } = "ApiKey"; // ApiKey, OAuth2, EImzo, SDK

        // Genel
        public string? ApiBaseUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ApiSecret { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }

        // Bazı sağlayıcılar için ek alanlar
        public string? FirmCode { get; set; }   // DİA/Mikro/Logo
        public int? Year { get; set; }          // Mikro
        public string? Extra1 { get; set; }     // Gerekirse
        public string? Extra2 { get; set; }

        // OAuth2 alanları
        public string? TokenUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? AccessToken { get; set; }
        public string? GrantType { get; set; }
        public string? ApiKeyHeaderName { get; set; }

        // Eski alanlar (geriye uyumluluk için)
        public string? WebhookSecret { get; set; }
        public string? VknTckn { get; set; }
        public string? Title { get; set; }
        public string? BranchCode { get; set; }
        public SignMode SignMode { get; set; } = SignMode.ProviderSign;
        public int TimeoutSec { get; set; } = 30;
        public int? RetryCountOverride { get; set; }
        public int? CircuitTripThreshold { get; set; }
    }
}
