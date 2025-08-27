using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Logo entegratör adapter'ı - Logo Tiger3 ERP entegrasyonu
/// </summary>
public class LogoProvider : IInvoiceProvider
{
    private readonly ILogger<LogoProvider> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public LogoProvider(ILogger<LogoProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Entegratör adı
    /// </summary>
    public string Name => "logo";

    /// <summary>
    /// Fatura gönderimi - Logo API'sine gönderir
    /// </summary>
    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Logo fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}, IdempotencyKey: {IdempotencyKey}", 
            envelope.InvoiceId, idempotencyKey);

        try
        {
            // Dev ortamında mock gönderim
            await Task.Delay(150, ct); // Simüle edilmiş API çağrısı

            // Mock başarılı yanıt
            var mockResponse = new
            {
                success = true,
                invoiceId = $"LOGO-{Guid.NewGuid():N}",
                status = "ACCEPTED",
                message = "Fatura başarıyla alındı",
                timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Logo fatura gönderimi tamamlandı. ProviderInvoiceId: {ProviderInvoiceId}", 
                mockResponse.invoiceId);

            return new ProviderSendResult(
                Success: true,
                ProviderInvoiceId: mockResponse.invoiceId,
                ProviderStatus: mockResponse.status,
                RawResponse: System.Text.Json.JsonSerializer.Serialize(mockResponse),
                ErrorCode: null,
                ErrorMessage: null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logo fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "LOGO_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    /// <summary>
    /// Webhook imza doğrulama - Logo webhook'larının güvenliğini kontrol eder
    /// </summary>
    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Logo webhook imza doğrulaması. Payload length: {PayloadLength}, Signature: {Signature}", 
            payload.Length, signature);

        // Dev ortamında mock doğrulama
        return true;
    }
}
