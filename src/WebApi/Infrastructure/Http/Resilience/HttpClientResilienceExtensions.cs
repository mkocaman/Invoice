// Türkçe: Tüm HttpClient'lara Polly tabanlı retry + circuit breaker ekleyen eklenti.
// Not: Named HttpClient kullanıyorsanız AddHttpClient("PROVIDER_X", ...) çağrısından sonra .AddResiliencePolicies() zincirleyin.
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace WebApi.Infrastructure.Http.Resilience
{
    public static class HttpClientResilienceExtensions
    {
        public static IHttpClientBuilder AddResiliencePolicies(this IHttpClientBuilder builder)
        {
            // Türkçe: Fast-transient hatalar için exponential backoff retry
            var retry = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))
                );

            // Türkçe: Aşırı hata durumunda devre kesici (circuit breaker)
            var breaker = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                );

            return builder.AddPolicyHandler(retry)
                          .AddPolicyHandler(breaker);
        }
    }
}
