using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Mock webhook imza doğrulama servisi (geliştirme ortamı için)
/// </summary>
public class MockWebhookSignatureValidator : IWebhookSignatureValidator
{
    /// <summary>
    /// Webhook imzasını doğrular (mock implementasyon)
    /// </summary>
    /// <param name="payload">Webhook gövdesi</param>
    /// <param name="signature">İmza</param>
    /// <param name="secret">Gizli anahtar</param>
    /// <param name="timestamp">Zaman damgası (opsiyonel)</param>
    /// <returns>İmza geçerli mi? (mock'ta her zaman true)</returns>
    public bool ValidateSignature(string payload, string signature, string secret, string? timestamp = null)
    {
        // Türkçe: Mock implementasyon - gerçek ortamda HMAC-SHA256 ile imza kontrolü yapılır
        return true;
    }
}
