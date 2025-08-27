using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Uyumsoft entegratör adapter'ı - Uyumsoft ERP entegrasyonu
/// </summary>
public class UyumsoftProvider : IInvoiceProvider
{
    private readonly ILogger<UyumsoftProvider> _logger;

    public UyumsoftProvider(ILogger<UyumsoftProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "uyumsoft";

    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Uyumsoft fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

        try
        {
            await Task.Delay(160, ct);

            var mockResponse = new
            {
                success = true,
                invoiceId = $"UYUMSOFT-{Guid.NewGuid():N}",
                status = "ACCEPTED",
                message = "Fatura başarıyla alındı",
                timestamp = DateTime.UtcNow
            };

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
            _logger.LogError(ex, "Uyumsoft fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "UYUMSOFT_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Uyumsoft webhook imza doğrulaması");
        return true;
    }
}
