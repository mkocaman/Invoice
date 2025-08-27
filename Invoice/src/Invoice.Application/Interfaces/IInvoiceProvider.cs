using Invoice.Application.Models;

namespace Invoice.Application.Interfaces;

/// <summary>
/// Her entegratör için zorunlu sözleşme. Fatura gönderimi ve webhook doğrulama.
/// </summary>
public interface IInvoiceProvider
{
    /// <summary>
    /// Fatura gönderimi - entegratöre fatura verilerini gönderir
    /// </summary>
    /// <param name="envelope">Fatura zarfı (müşteri, satır kalemleri, EŞÜ verileri)</param>
    /// <param name="idempotencyKey">İdempotency anahtarı</param>
    /// <param name="ct">İptal token'ı</param>
    /// <returns>Gönderim sonucu</returns>
    Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct);
    
    /// <summary>
    /// Webhook imza doğrulama - entegratörden gelen webhook'ların güvenliğini kontrol eder
    /// </summary>
    /// <param name="payload">Webhook gövdesi</param>
    /// <param name="signature">İmza</param>
    /// <param name="timestamp">Zaman damgası (opsiyonel)</param>
    /// <returns>İmza geçerli mi?</returns>
    bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null);
    
    /// <summary>
    /// Entegratör adı (foriba/logo/mikro/...)
    /// </summary>
    string Name { get; }
}

/// <summary>
/// Entegratör gönderim sonucu
/// </summary>
/// <param name="Success">İşlem başarılı mı?</param>
/// <param name="ProviderInvoiceId">Entegratör fatura ID'si</param>
/// <param name="ProviderStatus">Entegratör durumu</param>
/// <param name="RawResponse">Ham yanıt</param>
/// <param name="ErrorCode">Hata kodu</param>
/// <param name="ErrorMessage">Hata mesajı</param>
public sealed record ProviderSendResult(
    bool Success,
    string? ProviderInvoiceId,
    string? ProviderStatus,
    string? RawResponse,
    string? ErrorCode,
    string? ErrorMessage);
