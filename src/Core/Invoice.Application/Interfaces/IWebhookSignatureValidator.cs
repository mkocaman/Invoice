namespace Invoice.Application.Interfaces;

/// <summary>
/// Webhook imza doğrulama servisi sözleşmesi
/// </summary>
public interface IWebhookSignatureValidator
{
    /// <summary>
    /// Webhook imzasını doğrular
    /// </summary>
    /// <param name="payload">Webhook gövdesi</param>
    /// <param name="signature">İmza</param>
    /// <param name="secret">Gizli anahtar</param>
    /// <param name="timestamp">Zaman damgası (opsiyonel)</param>
    /// <returns>İmza geçerli mi?</returns>
    bool ValidateSignature(string payload, string signature, string secret, string? timestamp = null);
}
