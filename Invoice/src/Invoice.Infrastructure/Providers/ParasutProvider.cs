using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Parasut entegratör adapter'ı - Parasut ERP entegrasyonu
/// </summary>
public class ParasutProvider : IInvoiceProvider
{
    private readonly ILogger<ParasutProvider> _logger;

    public ParasutProvider(ILogger<ParasutProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "parasut";

    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Parasut fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

        try
        {
            await Task.Delay(120, ct);

            var mockResponse = new
            {
                success = true,
                invoiceId = $"PARASUT-{Guid.NewGuid():N}",
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
            _logger.LogError(ex, "Parasut fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "PARASUT_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Parasut webhook imza doğrulaması");
        return true;
    }
}
