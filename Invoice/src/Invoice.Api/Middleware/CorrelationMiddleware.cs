using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace Invoice.Api.Middleware;

/// <summary>
/// Korelasyon ID ve İdempotency Key middleware'i
/// </summary>
public class CorrelationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationMiddleware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Sonraki middleware</param>
    /// <param name="logger">Logger</param>
    public CorrelationMiddleware(RequestDelegate next, ILogger<CorrelationMiddleware> logger)
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
        // Korelasyon ID'yi al veya oluştur
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() 
            ?? Guid.NewGuid().ToString();
        
        // Activity'e korelasyon ID'yi ekle
        Activity.Current?.SetTag("correlation.id", correlationId);
        
        // Response header'a korelasyon ID'yi ekle
        context.Response.Headers["X-Correlation-Id"] = correlationId;
        
        // İdempotency Key kontrolü (sadece POST istekleri için)
        if (context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(idempotencyKey))
            {
                _logger.LogWarning("POST isteği için Idempotency-Key header'ı eksik. CorrelationId: {CorrelationId}", correlationId);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Idempotency-Key header'ı zorunludur");
                return;
            }
            
            // İdempotency Key'i request scope'a ekle
            context.Items["IdempotencyKey"] = idempotencyKey;
            
            _logger.LogInformation("İstek işleniyor. CorrelationId: {CorrelationId}, IdempotencyKey: {IdempotencyKey}", 
                correlationId, idempotencyKey);
        }
        else
        {
            _logger.LogInformation("İstek işleniyor. CorrelationId: {CorrelationId}", correlationId);
        }
        
        await _next(context);
    }
}
