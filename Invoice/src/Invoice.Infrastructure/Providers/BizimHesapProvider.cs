using Invoice.Application.Interfaces;
using Invoice.Application.Models;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// BizimHesap entegratör adapter'ı - BizimHesap ERP entegrasyonu
/// </summary>
public class BizimHesapProvider : IInvoiceProvider
{
    private readonly ILogger<BizimHesapProvider> _logger;

    public BizimHesapProvider(ILogger<BizimHesapProvider> logger)
    {
        _logger = logger;
    }

    public string Name => "bizimhesap";

    public async Task<ProviderSendResult> SendAsync(InvoiceEnvelope envelope, string idempotencyKey, CancellationToken ct)
    {
        _logger.LogInformation("BizimHesap fatura gönderimi başlatılıyor. InvoiceId: {InvoiceId}", envelope.InvoiceId);

        try
        {
            await Task.Delay(80, ct);

            var mockResponse = new
            {
                success = true,
                invoiceId = $"BIZIMHESAP-{Guid.NewGuid():N}",
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
            _logger.LogError(ex, "BizimHesap fatura gönderimi hatası. InvoiceId: {InvoiceId}", envelope.InvoiceId);
            
            return new ProviderSendResult(
                Success: false,
                ProviderInvoiceId: null,
                ProviderStatus: "ERROR",
                RawResponse: null,
                ErrorCode: "BIZIMHESAP_ERROR",
                ErrorMessage: ex.Message
            );
        }
    }

    public bool VerifyWebhookSignature(string payload, string signature, string? timestamp = null)
    {
        _logger.LogDebug("BizimHesap webhook imza doğrulaması");
        return true;
    }
}
