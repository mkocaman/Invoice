// Türkçe: Uygulama başlarken kritik configuration/sır doğrulaması.
using Microsoft.Extensions.Options;
using Infrastructure.Options;

namespace WebApi.Infrastructure.StartupValidation
{
    public static class StartupValidators
    {
        public static void ValidateProviderSecrets(ProviderSecretOptions secrets)
        {
            // Türkçe: Prod/uat için asgari alanlar boş olmamalı — dev'de uyarı verilebilir.
            if (string.IsNullOrWhiteSpace(secrets.Foriba.Username) || string.IsNullOrWhiteSpace(secrets.Foriba.Password))
                throw new InvalidOperationException("Foriba kimlik bilgileri eksik (Username/Password).");
            if (string.IsNullOrWhiteSpace(secrets.Logo.ClientId) || string.IsNullOrWhiteSpace(secrets.Logo.ClientSecret))
                throw new InvalidOperationException("Logo kimlik bilgileri eksik (ClientId/ClientSecret).");
        }
    }
}
