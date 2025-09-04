// Türkçe: Basit oran sınırlama (rate limit) - IP veya token başına.
// Not: Geliştirmenin erken safhasında DoS/yanlış yapılandırma etkisini azaltır.
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace WebApi.Infrastructure.RateLimiting
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddBasicRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("RateLimiting");
            var permitLimit = section.GetValue<int?>("PermitLimit") ?? 60;       // dk başına 60
            var windowSeconds = section.GetValue<int?>("WindowSeconds") ?? 60;   // 60 sn pencere
            var queueLimit = section.GetValue<int?>("QueueLimit") ?? 0;

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    // Türkçe: IP bazlı bölümlendirme (istersen bearer/sub bazlı da yapılabilir)
                    var key = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    return RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = permitLimit,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        QueueLimit = queueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    });
                });
            });

            return services;
        }
    }
}
