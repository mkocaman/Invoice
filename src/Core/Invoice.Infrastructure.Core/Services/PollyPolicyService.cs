using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using System.Net;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Polly retry ve circuit breaker politikaları
/// </summary>
public sealed class PollyPolicyService
{
    private readonly ILogger<PollyPolicyService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public PollyPolicyService(ILogger<PollyPolicyService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// HTTP retry politikası oluşturur
    /// </summary>
    /// <returns>Retry policy</returns>
    public AsyncRetryPolicy<HttpResponseMessage> CreateRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)), // 1s, 2s, 5s
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning(
                        "HTTP isteği başarısız oldu. Deneme: {RetryAttempt}, Bekleme: {WaitTime}ms, Hata: {Error}",
                        retryAttempt, timespan.TotalMilliseconds, outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                });
    }

    /// <summary>
    /// Circuit breaker politikası oluşturur
    /// </summary>
    /// <returns>Circuit breaker policy</returns>
    public AsyncCircuitBreakerPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 10,
                durationOfBreak: TimeSpan.FromSeconds(60),
                onBreak: (outcome, timespan) =>
                {
                    _logger.LogWarning("Circuit breaker açıldı. Süre: {Duration}ms", timespan.TotalMilliseconds);
                },
                onReset: () =>
                {
                    _logger.LogInformation("Circuit breaker kapandı");
                },
                onHalfOpen: () =>
                {
                    _logger.LogInformation("Circuit breaker yarı açık durumda");
                });
    }

    /// <summary>
    /// Kombine policy oluşturur (retry + circuit breaker)
    /// </summary>
    /// <returns>Kombine policy</returns>
    public IAsyncPolicy<HttpResponseMessage> CreateCombinedPolicy()
    {
        var retryPolicy = CreateRetryPolicy();
        var circuitBreakerPolicy = CreateCircuitBreakerPolicy();

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }

    /// <summary>
    /// 429 (Too Many Requests) için özel retry politikası
    /// </summary>
    /// <returns>429 retry policy</returns>
    public AsyncRetryPolicy<HttpResponseMessage> CreateRateLimitRetryPolicy()
    {
        return Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, context) =>
                {
                    // Retry-After header'ını kontrol et
                    if (context.TryGetValue("response", out var responseObj) && responseObj is HttpResponseMessage response)
                    {
                        if (response.Headers.RetryAfter?.Delta.HasValue == true)
                        {
                            return response.Headers.RetryAfter.Delta.Value;
                        }
                    }
                    
                    // Varsayılan exponential backoff
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    _logger.LogWarning(
                        "Rate limit aşıldı. Deneme: {RetryAttempt}, Bekleme: {WaitTime}ms",
                        retryAttempt, timespan.TotalMilliseconds);
                });
    }
}
