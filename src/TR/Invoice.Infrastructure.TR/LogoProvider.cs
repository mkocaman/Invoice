using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Providers
{
    /// <summary>
    /// Logo entegratörü için provider
    /// </summary>
    public partial class LogoProvider : IInvoiceProvider
    {
        private readonly ILogger<LogoProvider> _logger;
        private readonly IInvoiceUblService _ublService;

        /// <summary>
        /// Constructor
        /// </summary>
        public LogoProvider(ILogger<LogoProvider> logger, IInvoiceUblService ublService)
        {
            _logger = logger;
            _ublService = ublService;
        }

        /// <summary>
        /// Provider tipi
        /// </summary>
        public ProviderType ProviderType => ProviderType.LOGO;

        /// <summary>
        /// Fatura gönderir (minimal mock implementasyon)
        /// </summary>
        public async Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Logo'ya fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
                envelope.InvoiceNumber, envelope.TenantId);

            try
            {
                // UBL XML oluştur
                var ublXml = _ublService.BuildUblXml(envelope);
                
                // Mock payload oluştur
                var payload = $"Logo payload for invoice {envelope.InvoiceNumber} - Total: {envelope.TotalAmount} {envelope.Currency}";
                
                _logger.LogInformation("Logo'ya fatura gönderildi. Invoice Number: {InvoiceNumber}, Payload: {Payload}", 
                    envelope.InvoiceNumber, payload);

                return new ProviderSendResult(
                    Success: true,
                    Provider: ProviderType,
                    ProviderReferenceNumber: $"LOGO-{Guid.NewGuid():N}",
                    ProviderResponseMessage: "Mock başarılı gönderim",
                    UblXml: ublXml,
                    ErrorCode: null,
                    ErrorMessage: null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logo'ya fatura gönderilirken hata. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
                
                return ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "MOCK_ERROR",
                    errorMessage: $"Mock hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Webhook imza doğrulama (mock implementasyon)
        /// </summary>
        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            _logger.LogDebug("Logo webhook imza doğrulaması. Body length: {BodyLength}", body.Length);
            
            // Mock doğrulama - gerçek implementasyonda HMAC-SHA256 ile imza kontrolü yapılır
            return true;
        }
    }
}
