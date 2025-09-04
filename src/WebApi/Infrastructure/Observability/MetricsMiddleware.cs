// Türkçe: İstek bazlı metrikleri toplar (istek sayısı, süre, hata oranı)
using System.Diagnostics;
using Prometheus;

namespace WebApi.Infrastructure.Observability;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly Counter RequestCounter = Metrics.CreateCounter("invoiceapi_requests_total", "Toplam istek sayısı", new CounterConfiguration { LabelNames = new[] { "method", "endpoint", "status" } });
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram("invoiceapi_request_duration_seconds", "İstek süreleri", new HistogramConfiguration { LabelNames = new[] { "method", "endpoint" } });

    public MetricsMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
            var path = context.Request.Path.Value ?? "/";
            RequestCounter.WithLabels(context.Request.Method, path, context.Response.StatusCode.ToString()).Inc();
        }
        catch
        {
            var path = context.Request.Path.Value ?? "/";
            RequestCounter.WithLabels(context.Request.Method, path, "500").Inc();
            throw;
        }
        finally
        {
            sw.Stop();
            var path = context.Request.Path.Value ?? "/";
            RequestDuration.WithLabels(context.Request.Method, path).Observe(sw.Elapsed.TotalSeconds);
        }
    }
}
