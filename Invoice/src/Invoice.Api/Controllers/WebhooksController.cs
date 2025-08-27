using Microsoft.AspNetCore.Mvc;
using Invoice.Application.Interfaces;
using Invoice.Application.Helpers;

namespace Invoice.Api.Controllers;

/// <summary>
/// Webhook endpoint'leri
/// </summary>
[ApiController]
[Route("webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly ILogger<WebhooksController> _logger;
    private readonly IWebhookSignatureValidator _signatureValidator;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="signatureValidator">İmza doğrulayıcı</param>
    public WebhooksController(ILogger<WebhooksController> logger, IWebhookSignatureValidator signatureValidator)
    {
        _logger = logger;
        _signatureValidator = signatureValidator;
    }

    /// <summary>
    /// Webhook endpoint'i (tüm provider'lar için)
    /// </summary>
    /// <param name="provider">Provider adı</param>
    /// <returns>İşlem sonucu</returns>
    [HttpPost("integrators/{provider}")]
    public async Task<IActionResult> Webhook(string provider, [FromBody] object payload)
    {
        try
        {
            // Header'ları al
            var signature = Request.Headers["X-Signature"].FirstOrDefault();
            var timestamp = Request.Headers["X-Timestamp"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
            {
                _logger.LogWarning("Webhook: Gerekli header'lar eksik. Provider: {Provider}", provider);
                return Unauthorized("Missing required headers");
            }

            // Request body'yi al
            var body = await GetRequestBodyAsync();

            // İmza doğrulama
            var isValid = await _signatureValidator.ValidateSignatureAsync(body, signature, timestamp);
            if (!isValid)
            {
                _logger.LogWarning("Webhook: İmza doğrulaması başarısız. Provider: {Provider}, IP: {IP}", 
                    provider, GetClientIP());
                return Unauthorized("Invalid signature");
            }

            // Webhook işleme (şimdilik basit log)
            _logger.LogInformation("Webhook alındı. Provider: {Provider}, Payload: {Payload}", 
                provider, MaskingHelper.MaskVkn(payload.ToString()));

            // Başarılı yanıt
            return Ok(new { status = "processed", provider = provider });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook işleme sırasında hata oluştu. Provider: {Provider}", provider);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Request body'yi alır
    /// </summary>
    /// <returns>Body içeriği</returns>
    private async Task<string> GetRequestBodyAsync()
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;
        return body;
    }

    /// <summary>
    /// Client IP adresini alır
    /// </summary>
    /// <returns>IP adresi</returns>
    private string GetClientIP()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
