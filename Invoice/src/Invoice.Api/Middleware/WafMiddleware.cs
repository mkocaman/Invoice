using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Invoice.Api.Middleware;

/// <summary>
/// Web Application Firewall (WAF) middleware'i
/// </summary>
public class WafMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WafMiddleware> _logger;
    private readonly string[] _riskPatterns = { "<script", "union select", "drop table", "exec(", "eval(", "javascript:" };

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Sonraki middleware</param>
    /// <param name="logger">Logger</param>
    public WafMiddleware(RequestDelegate next, ILogger<WafMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middleware işlemi
    /// </summary>
    /// <param name="context">HTTP context</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Header uzunluk kontrolü
            if (!ValidateHeaders(context))
            {
                _logger.LogWarning("WAF: Çok uzun header tespit edildi. IP: {IP}", GetClientIP(context));
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid request headers");
                
                // WAF metrik sayacını artır
                var meter = new System.Diagnostics.Metrics.Meter("Invoice.Api");
                var wafBlockedCounter = meter.CreateCounter<long>("waf_blocked_total");
                wafBlockedCounter.Add(1);
                
                return;
            }

            // Content-Type kontrolü
            if (!ValidateContentType(context))
            {
                _logger.LogWarning("WAF: Geçersiz Content-Type. IP: {IP}, Content-Type: {ContentType}", 
                    GetClientIP(context), context.Request.ContentType);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid content type");
                return;
            }

            // Riskli pattern kontrolü
            if (await ContainsRiskPatterns(context))
            {
                _logger.LogWarning("WAF: Riskli pattern tespit edildi. IP: {IP}", GetClientIP(context));
                context.Response.StatusCode = 406;
                await context.Response.WriteAsync("Request contains forbidden patterns");
                
                // WAF metrik sayacını artır
                var meter = new System.Diagnostics.Metrics.Meter("Invoice.Api");
                var wafBlockedCounter = meter.CreateCounter<long>("waf_blocked_total");
                wafBlockedCounter.Add(1);
                
                return;
            }

            // JSON gövde kontrolü (sadece POST/PUT istekleri için)
            if (IsJsonRequest(context) && !await ValidateJsonBody(context))
            {
                _logger.LogWarning("WAF: Geçersiz JSON gövde. IP: {IP}", GetClientIP(context));
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid JSON body");
                return;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WAF middleware'de hata oluştu. IP: {IP}", GetClientIP(context));
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

    /// <summary>
    /// Header uzunluklarını doğrular
    /// </summary>
    private bool ValidateHeaders(HttpContext context)
    {
        const int maxHeaderLength = 8192; // 8KB
        const int maxHeaderCount = 50;

        if (context.Request.Headers.Count > maxHeaderCount)
            return false;

        foreach (var header in context.Request.Headers)
        {
            if (header.Value.ToString().Length > maxHeaderLength)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Content-Type'ı doğrular
    /// </summary>
    private bool ValidateContentType(HttpContext context)
    {
        if (string.IsNullOrEmpty(context.Request.ContentType))
            return true; // Content-Type yoksa geçerli

        var allowedTypes = new[] { "application/json", "application/x-www-form-urlencoded", "text/plain" };
        return allowedTypes.Any(type => context.Request.ContentType.StartsWith(type, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Riskli pattern'leri kontrol eder
    /// </summary>
    private async Task<bool> ContainsRiskPatterns(HttpContext context)
    {
        // URL kontrolü
        var url = context.Request.Path + context.Request.QueryString;
        if (_riskPatterns.Any(pattern => url.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            return true;

        // Header kontrolü
        foreach (var header in context.Request.Headers)
        {
            var headerValue = header.Value.ToString();
            if (_riskPatterns.Any(pattern => headerValue.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        // Body kontrolü (sadece küçük gövde için)
        if (context.Request.ContentLength > 0 && context.Request.ContentLength < 10240) // 10KB
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (_riskPatterns.Any(pattern => body.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }

    /// <summary>
    /// JSON isteği olup olmadığını kontrol eder
    /// </summary>
    private bool IsJsonRequest(HttpContext context)
    {
        return context.Request.Method == "POST" || context.Request.Method == "PUT" || context.Request.Method == "PATCH";
    }

    /// <summary>
    /// JSON gövdeyi doğrular
    /// </summary>
    private async Task<bool> ValidateJsonBody(HttpContext context)
    {
        if (context.Request.ContentLength == 0)
            return true;

        try
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return true;

            // JSON parse testi
            JsonDocument.Parse(body);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Client IP adresini alır
    /// </summary>
    private string GetClientIP(HttpContext context)
    {
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
