// Türkçe: Sadece CSMS'in verdiği token'ların geçmesine izin veren yetkilendirme politikası.
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Infrastructure.Security.Policies
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCsmsAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Türkçe: scope claim'i "csms.invoice" olan token'lara izin ver.
                options.AddPolicy("CsmsOnly", policy =>
                    policy.RequireAuthenticatedUser()
                          .RequireClaim("scope", "csms.invoice"));
            });

            return services;
        }
    }
}
