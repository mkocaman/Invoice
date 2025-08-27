using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Netsis entegratör adapter'ı - Netsis ERP entegrasyonu
/// </summary>
public class NetsisProvider : IInvoiceProvider
{
    private readonly ILogger<NetsisProvider> _logger;

    public NetsisProvider(ILogger<NetsisProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "netsis";

    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Netsis fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

        try
        {
            await Task.Delay(70, ct);

            var mockResponse = new
            {
                success = true,
                invoiceId = $"NETSIS-{Guid.NewGuid():N}",
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
            _logger.LogError(ex, "Netsis fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "NETSIS_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Netsis webhook imza doğrulaması");
        return true;
    }
}
