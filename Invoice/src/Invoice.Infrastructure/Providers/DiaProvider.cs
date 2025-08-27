using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Dia entegratör adapter'ı - Dia ERP entegrasyonu
/// </summary>
public class DiaProvider : IInvoiceProvider
{
    private readonly ILogger<DiaProvider> _logger;

    public DiaProvider(ILogger<DiaProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "dia";

    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("Dia fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

        try
        {
            await Task.Delay(100, ct);

            var mockResponse = new
            {
                success = true,
                invoiceId = $"DIA-{Guid.NewGuid():N}",
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
            _logger.LogError(ex, "Dia fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "DIA_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("Dia webhook imza doğrulaması");
        return true;
    }
}
