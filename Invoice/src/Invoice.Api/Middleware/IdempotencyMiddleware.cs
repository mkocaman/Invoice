using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace Invoice.Api.Middleware;

/// <summary>
/// İdempotency middleware'i - aynı isteğin tekrar işlenmesini engeller
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="next">Sonraki middleware</param>
    /// <param name="logger">Logger</param>
    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
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
        // Sadece POST, PUT, PATCH, DELETE isteklerinde çalış
        if (!IsModifyingRequest(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // İdempotency-Key header'ını kontrol et
        var idempotencyKey = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrEmpty(idempotencyKey))
        {
            _logger.LogWarning("İdempotency-Key header'ı eksik. Method: {Method}, Path: {Path}", 
                context.Request.Method, context.Request.Path);
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Idempotency-Key header'ı zorunludur");
            return;
        }

        // DbContext'i al
        var dbContext = context.RequestServices.GetRequiredService<InvoiceDbContext>();
        
        try
        {
            // Aynı anahtar daha önce kullanılmış mı kontrol et
            var existingKey = await dbContext.IdempotencyKeys
                .FirstOrDefaultAsync(k => k.Key == idempotencyKey);

            if (existingKey != null)
            {
                _logger.LogWarning("İdempotency anahtarı zaten kullanılmış. Key: {Key}, FirstSeen: {FirstSeen}", 
                    idempotencyKey, existingKey.FirstSeenAt);
                
                context.Response.StatusCode = 409; // Conflict
                await context.Response.WriteAsync("Bu işlem zaten işlendi");
                return;
            }

            // Yeni idempotency anahtarı oluştur
            var newKey = new Domain.Entities.IdempotencyKey
            {
                Id = Guid.NewGuid(),
                Key = idempotencyKey,
                Hash = await CalculateRequestHash(context.Request),
                Method = context.Request.Method,
                Path = context.Request.Path,
                FirstSeenAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // 24 saat geçerli
                TenantId = GetTenantId(context) // Mevcut tenant ID'yi al
            };

            // Veritabanına kaydet
            dbContext.IdempotencyKeys.Add(newKey);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Yeni idempotency anahtarı oluşturuldu. Key: {Key}, Method: {Method}, Path: {Path}", 
                idempotencyKey, context.Request.Method, context.Request.Path);

            // İsteği devam ettir
            await _next(context);

            // Başarılı yanıt durumunda sonuç kodu kaydet
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                newKey.LastStatusCode = context.Response.StatusCode;
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İdempotency middleware'de hata oluştu. Key: {Key}", idempotencyKey);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Internal server error");
        }
    }

    /// <summary>
    /// İsteğin değiştirici bir istek olup olmadığını kontrol eder
    /// </summary>
    /// <param name="method">HTTP metodu</param>
    /// <returns>Değiştirici istek mi?</returns>
    private bool IsModifyingRequest(string method)
    {
        return method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) ||
               method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// İstek gövdesinin hash'ini hesaplar
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Hash değeri</returns>
    private async Task<string> CalculateRequestHash(HttpRequest request)
    {
        try
        {
            if (request.ContentLength == 0)
                return string.Empty;

            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;

            if (string.IsNullOrEmpty(body))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(body));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Tenant ID'yi alır (mevcut implementasyona göre)
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Tenant ID</returns>
    private string GetTenantId(HttpContext context)
    {
        // Mevcut tenant ID alma mantığına göre implement edin
        // Şimdilik varsayılan değer
        return context.Request.Headers["X-Tenant-Id"].FirstOrDefault() ?? "default";
    }
}
