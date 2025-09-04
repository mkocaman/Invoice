// Türkçe: Sağlayıcı çağrılarını Polly ile saran invoker; fallback/failover destekler.
using Invoice.Application.Interfaces;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Providers.Selection
{
    public interface IProviderInvoker
    {
        Task<T> InvokeAsync<T>(
            IInvoiceProvider primary,
            Func<IInvoiceProvider, Task<T>> action,
            IEnumerable<IInvoiceProvider>? fallbacks = null,
            CancellationToken ct = default);
    }

    public sealed class ProviderInvoker : IProviderInvoker
    {
        private readonly ILogger<ProviderInvoker> _logger;
        private readonly AsyncRetryPolicy _retry;
        private readonly AsyncCircuitBreakerPolicy _breaker;

        public ProviderInvoker(ILogger<ProviderInvoker> logger)
        {
            _logger = logger;

            // Türkçe: Kısa retry
            _retry = Policy.Handle<Exception>()
                           .WaitAndRetryAsync(2, a => TimeSpan.FromMilliseconds(200 * a));

            // Türkçe: Basit circuit breaker
            _breaker = Policy.Handle<Exception>()
                             .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

        public async Task<T> InvokeAsync<T>(
            IInvoiceProvider primary,
            Func<IInvoiceProvider, Task<T>> action,
            IEnumerable<IInvoiceProvider>? fallbacks = null,
            CancellationToken ct = default)
        {
            // Türkçe: Önce primary üzerinde dener; olmazsa sırayla fallback'lere geçer.
            var chain = new List<IInvoiceProvider> { primary };
            if (fallbacks != null) chain.AddRange(fallbacks);

            Exception? last = null;

            foreach (var p in chain)
            {
                try
                {
                    // Türkçe: Retry + breaker kombinasyonu
                    return await _retry.WrapAsync(_breaker).ExecuteAsync(() => action(p));
                }
                catch (Exception ex)
                {
                    last = ex;
                    _logger.LogWarning(ex, "Sağlayıcı çağrısı başarısız: {ProviderType}", p.ProviderType);
                }
            }

            throw last ?? new InvalidOperationException("Sağlayıcı zinciri başarısız.");
        }
    }
}
