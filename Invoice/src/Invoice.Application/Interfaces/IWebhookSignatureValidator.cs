namespace Invoice.Application.Interfaces;

/// <summary>
/// Webhook imza doğrulama arayüzü
/// </summary>
public interface IWebhookSignatureValidator
{
    /// <summary>
    /// Webhook imzasını doğrular
    /// </summary>
    /// <param name="body">Request body</param>
    /// <param name="signature">X-Signature header değeri</param>
    /// <param name="timestamp">X-Timestamp header değeri</param>
    /// <returns>Doğrulama sonucu</returns>
    Task<bool> ValidateSignatureAsync(string body, string signature, string timestamp);
    
    /// <summary>
    /// Provider adını döner
    /// </summary>
    string ProviderName { get; }
}
