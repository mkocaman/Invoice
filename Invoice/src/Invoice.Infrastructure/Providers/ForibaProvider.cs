using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Foriba entegratör adapter'ı - E-Arşiv ve E-Fatura entegrasyonu
/// </summary>
public class ForibaProvider : IInvoiceProvider
{
    private readonly ILogger<ForibaProvider> _logger;
    private readonly IInvoiceUblService _ublService;
    private readonly ISigningService _signingService;
    private readonly IUblValidationService _validationService;

    /// <summary>
    /// Constructor
    /// </summary>
    public ForibaProvider(
        ILogger<ForibaProvider> logger,
        IInvoiceUblService ublService,
        ISigningService signingService,
        IUblValidationService validationService)
    {
        _logger = logger;
        _ublService = ublService;
        _signingService = signingService;
        _validationService = validationService;
    }

    /// <summary>
    /// Entegratör adı
    /// </summary>
    public string Name => "foriba";

    /// <summary>
    /// Fatura gönderimi - Foriba API'sine gönderir
    /// </summary>
    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Foriba fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}, IdempotencyKey: {IdempotencyKey}", 
            envelope.InvoiceId, idempotencyKey);

        try
        {
            // Dev ortamında mock gönderim
            await Task.Delay(200, ct); // Simüle edilmiş API çağrısı

            // Mock başarılı yanıt
            var mockResponse = new
            {
                success = true,
                invoiceId = $"FORIBA-{Guid.NewGuid():N}",
                status = "ACCEPTED",
                message = "Fatura başarıyla alındı",
                timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Foriba fatura gönderimi tamamlandı. ProviderInvoiceId: {ProviderInvoiceId}", 
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
            _logger.LogError(ex, "Foriba fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "FORIBA_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    /// <summary>
    /// Webhook imza doğrulama - Foriba webhook'larının güvenliğini kontrol eder
    /// </summary>
    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Foriba webhook imza doğrulaması. Payload length: {PayloadLength}, Signature: {Signature}", 
            payload.Length, signature);

        // Dev ortamında mock doğrulama
        // Gerçek implementasyonda HMAC-SHA256 ile imza kontrolü yapılır
        return true;
    }
}
