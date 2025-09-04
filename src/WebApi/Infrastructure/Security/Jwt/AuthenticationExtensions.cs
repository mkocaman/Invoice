// Türkçe: CSMS tarafından imzalanan JWT'leri doğrulayan eklenti.
// Not: Burada login/refresh yok. Sadece "CSMS → Invoice API" makine-makine doğrulaması var.
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Infrastructure.Security.Jwt
{
    public static class AuthenticationExtensions
    {
        // Türkçe: Uygulama ayarları için basit POCO
        public sealed class CsmsJwtOptions
        {
            public string? Issuer { get; set; }          // CSMS Issuer
            public string? Audience { get; set; } = "invoice-api"; // Bu servisin Audience'ı
            public string? SigningKey { get; set; }      // HMAC veya base64 RSA public key (senaryoya göre)
            public bool ValidateLifetime { get; set; } = true;
        }

        public static IServiceCollection AddCsmsJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Türkçe: "Security:Csms" altında konfigürasyon beklenir.
            var section = configuration.GetSection("Security:Csms");
            var options = section.Get<CsmsJwtOptions>() ?? new CsmsJwtOptions();

            // Türkçe: SigningKey zorunlu; CSMS tarafından sağlanır.
            if (string.IsNullOrWhiteSpace(options.SigningKey))
                throw new InvalidOperationException("Security:Csms:SigningKey yapılandırılmamış.");

            // Türkçe: HMAC senaryosu (örnek). RSA kullanılacaksa IssuerSigningKey yerine IssuerSigningKeys ile RSA public key verilir.
            var keyBytes = Encoding.UTF8.GetBytes(options.SigningKey);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.RequireHttpsMetadata = true;
                    o.SaveToken = false; // Türkçe: Token'ı server tarafında saklamayız.

                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = options.Issuer,
                        ValidateAudience = true,
                        ValidAudience = options.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = options.ValidateLifetime,
                        ClockSkew = TimeSpan.FromSeconds(30) // Türkçe: Küçük bir tolerans
                    };
                });

            return services;
        }
    }
}
