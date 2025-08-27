using System.Security.Cryptography;
using System.Text;
using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Webhook imza doğrulama servisi
/// </summary>
public class WebhookSignatureValidator : IWebhookSignatureValidator
{
    private readonly ILogger<WebhookSignatureValidator> _logger;
    private readonly string _secretKey;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="configuration">Configuration</param>
    public WebhookSignatureValidator(ILogger<WebhookSignatureValidator> logger, IConfiguration configuration)
    {
        _logger = logger;
        _secretKey = configuration["ApiSettings:WebhookSecret"] ?? "default-webhook-secret";
    }

    /// <summary>
    /// Provider adı
    /// </summary>
    public string ProviderName => "SharedSecret";

    /// <summary>
    /// Webhook imzasını doğrular
    /// </summary>
    /// <param name="body">Request body</param>
    /// <param name="signature">X-Signature header değeri</param>
    /// <param name="timestamp">X-Timestamp header değeri</param>
    /// <returns>Doğrulama sonucu</returns>
    public async Task<bool> ValidateSignatureAsync(string body, string signature, string timestamp)
    {
        try
        {
            // Timestamp kontrolü (5 dakika tolerans)
            if (!DateTime.TryParse(timestamp, out var requestTime))
            {
                _logger.LogWarning("Webhook: Geçersiz timestamp formatı. Timestamp: {Timestamp}", timestamp);
                return false;
            }

            var timeDiff = Math.Abs((DateTime.UtcNow - requestTime).TotalMinutes);
            if (timeDiff > 5)
            {
                _logger.LogWarning("Webhook: Timestamp çok eski. Fark: {TimeDiff} dakika", timeDiff);
                return false;
            }

            // HMAC-SHA256 hesaplama
            var expectedSignature = await CalculateHmacAsync(body, timestamp);
            
            // İmza karşılaştırması (timing attack'e karşı sabit zaman)
            var isValid = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(signature),
                Encoding.UTF8.GetBytes(expectedSignature));

            if (!isValid)
            {
                _logger.LogWarning("Webhook: İmza doğrulaması başarısız. Beklenen: {Expected}, Gelen: {Received}", 
                    expectedSignature, signature);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook imza doğrulama sırasında hata oluştu");
            return false;
        }
    }

    /// <summary>
    /// HMAC-SHA256 hesaplar
    /// </summary>
    /// <param name="body">Request body</param>
    /// <param name="timestamp">Timestamp</param>
    /// <returns>Base64 encoded signature</returns>
    private async Task<string> CalculateHmacAsync(string body, string timestamp)
    {
        var message = $"{timestamp}.{body}";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var keyBytes = Encoding.UTF8.GetBytes(_secretKey);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = await Task.Run(() => hmac.ComputeHash(messageBytes));
        return Convert.ToBase64String(hashBytes);
    }
}
