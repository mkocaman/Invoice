// Türkçe Açıklama:
// Her HTTP isteği için Correlation-Id (veya mevcutsa X-Correlation-Id) üretir,
// log kapsamına ve response header'a ekler. EF ve RabbitMQ logları da bu kapsamı görür.

using System.Diagnostics;

namespace WebApi.Infrastructure.Logging;

public class CorrelationMiddleware
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly RequestDelegate _next;
    public CorrelationMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext ctx, ILogger<CorrelationMiddleware> logger)
    {
        // İstekten geliyorsa kullan, yoksa üret
        var cid = ctx.Request.Headers.TryGetValue(HeaderName, out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : Guid.NewGuid().ToString("n");

        // Activity ile Trace/Span üret (OpenTelemetry yoksa bile .NET Activity iş görür)
        using var activity = new Activity("http-request");
        activity.SetIdFormat(ActivityIdFormat.W3C);
        activity.Start();
        activity.AddTag("correlation_id", cid);

        using (logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = cid,
            ["TraceId"] = Activity.Current?.TraceId.ToString()
        }))
        {
            ctx.Response.Headers[HeaderName] = cid;
            await _next(ctx);
        }
    }
}
