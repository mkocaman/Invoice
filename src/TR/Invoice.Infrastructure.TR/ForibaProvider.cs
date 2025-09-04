using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Application.Helpers;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.TR.Providers
{
    /// <summary>
    /// Foriba entegratörü için provider
    /// </summary>
    public partial class ForibaProvider : IInvoiceProvider
    {
        private readonly ILogger<ForibaProvider> _logger;
        private readonly IInvoiceUblService _ublService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ForibaProvider(ILogger<ForibaProvider> logger, IInvoiceUblService ublService)
        {
            _logger = logger;
            _ublService = ublService;
        }

        /// <summary>
        /// Provider tipi
        /// </summary>
        public ProviderType ProviderType => ProviderType.FORIBA;
        
        public string Key => "foriba";
        
        public string CountryCode => "TR";
        /// <summary>
        /// Fatura gönderir (minimal mock implementasyon)
        /// </summary>
        public Task<ProviderSendResult> SendInvoiceAsync(InvoiceEnvelope envelope, ProviderConfig config, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Foriba'ya fatura gönderiliyor. Invoice Number: {InvoiceNumber}, Tenant: {TenantId}", 
                envelope.InvoiceNumber, envelope.TenantId);

            try
            {
                // UBL XML oluştur
                var ublXml = _ublService.BuildUblXml(envelope);
                
                // Mock payload oluştur
                var payload = $"Foriba payload for invoice {envelope.InvoiceNumber} - Total: {envelope.TotalAmount} {envelope.Currency}";
                
                _logger.LogInformation("Foriba'ya fatura gönderildi. Invoice Number: {InvoiceNumber}, Payload: {Payload}", 
                    envelope.InvoiceNumber, payload);

                var resp = new ProviderSendResult(
                    Success: true,
                    Provider: ProviderType,
                    ProviderReferenceNumber: $"FORIBA-{Guid.NewGuid():N}",
                    ProviderResponseMessage: "Mock başarılı gönderim",
                    UblXml: ublXml,
                    ErrorCode: null,
                    ErrorMessage: null);
                return Task.FromResult(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Foriba'ya fatura gönderilirken hata. Invoice Number: {InvoiceNumber}", envelope.InvoiceNumber);
                
                var resp = ProviderSendResult.Failed(
                    provider: ProviderType,
                    errorCode: "MOCK_ERROR",
                    errorMessage: $"Mock hata: {ex.Message}");
                return Task.FromResult(resp);
            }
        }

        /// <summary>
        /// Webhook imza doğrulama (mock implementasyon)
        /// </summary>
        public bool VerifyWebhookSignature(IReadOnlyDictionary<string, string> headers, string body, ProviderConfig config)
        {
            _logger.LogDebug("Foriba webhook imza doğrulaması. Body length: {BodyLength}", body.Length);
            
            // Mock doğrulama - gerçek implementasyonda HMAC-SHA256 ile imza kontrolü yapılır
            return true;
        }


    }
}
