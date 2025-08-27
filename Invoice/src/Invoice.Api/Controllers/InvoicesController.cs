using Microsoft.AspNetCore.Mvc;
using Invoice.Application.Interfaces;
using Invoice.Application.Models;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Api.Controllers;

/// <summary>
/// Fatura işlemleri controller'ı - EŞÜ/şarj oturumu faturaları
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly ILogger<InvoicesController> _logger;
    private readonly IInvoiceProviderFactory _providerFactory;
    private readonly IInvoiceUblService _ublService;
    private readonly IUblValidationService _validationService;
    private readonly ISigningService _signingService;
    private readonly IProviderConfigurationService _providerConfigService;
    private readonly InvoiceDbContext _dbContext;

    /// <summary>
    /// Constructor
    /// </summary>
    public InvoicesController(
        ILogger<InvoicesController> logger,
        IInvoiceProviderFactory providerFactory,
        IInvoiceUblService ublService,
        IUblValidationService validationService,
        ISigningService signingService,
        IProviderConfigurationService providerConfigService,
        InvoiceDbContext dbContext)
    {
        _logger = logger;
        _providerFactory = providerFactory;
        _ublService = ublService;
        _validationService = validationService;
        _signingService = signingService;
        _providerConfigService = providerConfigService;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Şarj oturumu faturası oluştur ve gönder
    /// </summary>
    /// <param name="request">Şarj oturumu fatura isteği</param>
    /// <returns>Fatura gönderim sonucu</returns>
    [HttpPost("charge-session")]
    public async Task<IActionResult> CreateChargeSessionInvoice([FromBody] ChargeSessionRequest request)
    {
        _logger.LogInformation("Şarj oturumu faturası oluşturuluyor. TenantId: {TenantId}, ProviderKey: {ProviderKey}", 
            request.TenantId, request.ProviderKey);

        try
        {
            // Model validasyonu
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Geçersiz model durumu. Hatalar: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            // Provider konfigürasyonunu al
            var providerConfig = await _providerConfigService.GetProviderConfigurationAsync(request.ProviderKey);
            if (providerConfig == null)
            {
                _logger.LogWarning("Provider konfigürasyonu bulunamadı. TenantId: {TenantId}, ProviderKey: {ProviderKey}", 
                    request.TenantId, request.ProviderKey);
                return BadRequest(new { error = "Provider konfigürasyonu bulunamadı" });
            }

            // InvoiceEnvelope oluştur
            var envelope = await CreateInvoiceEnvelopeAsync(request, providerConfig);

            // SignMode'a göre işlem yap
            string signedContent;
            if (providerConfig.SignMode == SignMode.SelfSign)
            {
                // SelfSign: UBL üret → validasyon → imzala
                var ublXml = await _ublService.CreateUblXmlAsync(envelope);
                
                // XSD validasyonu (opsiyonel)
                var validationResult = await _validationService.ValidateUblAsync(ublXml);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("UBL validasyon hatası. Hatalar: {Errors}", 
                        string.Join(", ", validationResult.Errors));
                    return BadRequest(new { error = "UBL validasyon hatası", details = validationResult.Errors });
                }

                // XAdES-BES imzala
                signedContent = await _signingService.SignUblAsync(ublXml, "certificate.pfx", "password");
                signedContent = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(signedContent));
            }
            else
            {
                // ProviderSign: JSON format hazırla
                signedContent = System.Text.Json.JsonSerializer.Serialize(envelope);
            }

            // Provider adapter'ını al ve gönder
            var provider = _providerFactory.Resolve(request.ProviderKey);
            var result = await provider.SendAsync(envelope, request.IdempotencyKey, CancellationToken.None);

            // Fatura kaydını oluştur
            var invoice = new Domain.Entities.Invoice
            {
                Id = envelope.InvoiceId,
                InvoiceNumber = envelope.InvoiceNumber,
                InvoiceDate = envelope.InvoiceDate,
                DueDate = envelope.DueDate,
                Currency = envelope.Currency,
                SubTotal = envelope.SubTotal,
                TaxAmount = envelope.TaxAmount,
                TotalAmount = envelope.TotalAmount,
                Status = result.Success ? "ACCEPTED" : "ERROR",
                ProviderReferenceNumber = result.ProviderInvoiceId,
                ProviderResponseMessage = result.RawResponse,
                ProviderErrorCode = result.ErrorCode,
                ProviderErrorMessage = result.ErrorMessage,
                TenantId = request.TenantId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.Invoices.Add(invoice);
            await _dbContext.SaveChangesAsync();

            // IntegrationLog'a yaz
            await LogIntegrationAsync(request.TenantId, "CHARGE_SESSION_INVOICE", 
                result.Success ? "SUCCESS" : "ERROR", provider.Name, result.RawResponse);

            _logger.LogInformation("Şarj oturumu faturası tamamlandı. InvoiceId: {InvoiceId}, Success: {Success}", 
                envelope.InvoiceId, result.Success);

            return Accepted(new
            {
                invoiceId = envelope.InvoiceId,
                invoiceNumber = envelope.InvoiceNumber,
                providerInvoiceId = result.ProviderInvoiceId,
                status = result.Success ? "ACCEPTED" : "ERROR",
                message = result.Success ? "Fatura başarıyla gönderildi" : result.ErrorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Şarj oturumu faturası oluşturma hatası. TenantId: {TenantId}", request.TenantId);
            
            await LogIntegrationAsync(request.TenantId, "CHARGE_SESSION_INVOICE", "ERROR", 
                request.ProviderKey, ex.Message);

            return StatusCode(500, new { error = "Fatura oluşturma hatası", details = ex.Message });
        }
    }

    /// <summary>
    /// Toplu fatura gönderimi - rapor ID bazlı
    /// </summary>
    /// <param name="request">Toplu fatura isteği</param>
    /// <returns>Toplu gönderim sonucu</returns>
    [HttpPost("charge-batch")]
    public async Task<IActionResult> CreateChargeBatchInvoices([FromBody] ChargeBatchRequest request)
    {
        _logger.LogInformation("Toplu fatura gönderimi başlatılıyor. ReportId: {ReportId}, TenantId: {TenantId}", 
            request.ReportId, request.TenantId);

        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Rapor ID'yi kontrol et
            var report = await _dbContext.ReportIds
                .FirstOrDefaultAsync(r => r.Id == request.ReportId && r.TenantId == request.TenantId);

            if (report == null)
            {
                return BadRequest(new { error = "Rapor bulunamadı" });
            }

            // Şarj seanslarını al (son 7 gün)
            var sessions = await _dbContext.ChargeSessions
                .Where(cs => cs.TenantId == request.TenantId && 
                            cs.StartDate >= DateTime.UtcNow.AddDays(-7) &&
                            cs.Status == "COMPLETED")
                .ToListAsync();

            if (!sessions.Any())
            {
                return BadRequest(new { error = "Gönderilecek şarj seansı bulunamadı" });
            }

            // Her seans için envelope oluştur ve kuyruğa ekle
            foreach (var session in sessions)
            {
                // Mock kuyruk işlemi - gerçek implementasyonda message queue kullanılır
                await LogIntegrationAsync(request.TenantId, "CHARGE_BATCH_QUEUED", "INFO", 
                    request.ProviderKey, $"Seans {session.Id} kuyruğa eklendi");
            }

            _logger.LogInformation("Toplu fatura gönderimi planlandı. Seans sayısı: {SessionCount}", sessions.Count);

            return Accepted(new
            {
                reportId = request.ReportId,
                sessionCount = sessions.Count,
                message = $"{sessions.Count} seans kuyruğa eklendi"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Toplu fatura gönderimi hatası. ReportId: {ReportId}", request.ReportId);
            return StatusCode(500, new { error = "Toplu fatura gönderimi hatası", details = ex.Message });
        }
    }

    /// <summary>
    /// Fatura detaylarını getir
    /// </summary>
    /// <param name="id">Fatura ID'si</param>
    /// <returns>Fatura detayları</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        _logger.LogDebug("Fatura detayları alınıyor. InvoiceId: {InvoiceId}", id);

        try
        {
            var invoice = await _dbContext.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return NotFound(new { error = "Fatura bulunamadı" });
            }

            // Son log kayıtlarını al
            var logs = await _dbContext.IntegrationLogs
                .Where(l => l.CorrelationId == id.ToString())
                .OrderByDescending(l => l.CreatedAt)
                .Take(10)
                .ToListAsync();

            var response = new
            {
                invoiceId = invoice.Id,
                invoiceNumber = invoice.InvoiceNumber,
                invoiceDate = invoice.InvoiceDate,
                status = invoice.Status,
                providerStatus = invoice.ProviderReferenceNumber,
                uuid = invoice.Uuid,
                subTotal = invoice.SubTotal,
                taxAmount = invoice.TaxAmount,
                totalAmount = invoice.TotalAmount,
                currency = invoice.Currency,
                providerErrorCode = invoice.ProviderErrorCode,
                providerErrorMessage = invoice.ProviderErrorMessage,
                createdAt = invoice.CreatedAt,
                updatedAt = invoice.UpdatedAt,
                logs = logs.Select(l => new
                {
                    level = l.Level,
                    message = l.Message,
                    createdAt = l.CreatedAt,
                    operationType = l.OperationType
                })
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatura detayları alma hatası. InvoiceId: {InvoiceId}", id);
            return StatusCode(500, new { error = "Fatura detayları alma hatası", details = ex.Message });
        }
    }

    /// <summary>
    /// InvoiceEnvelope oluştur
    /// </summary>
    private async Task<InvoiceEnvelope> CreateInvoiceEnvelopeAsync(ChargeSessionRequest request, ProviderConfig config)
    {
        var envelope = new InvoiceEnvelope
        {
            InvoiceId = Guid.NewGuid(),
            InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}".Substring(0, 20),
            InvoiceDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30),
            Currency = "TRY",
            ExchangeRate = 1.0m,
            Customer = new CustomerInfo
            {
                CustomerId = request.Customer.CustomerId,
                Name = request.Customer.Name,
                TaxNumber = request.Customer.TaxNumber,
                TaxOffice = request.Customer.TaxOffice,
                Address = request.Customer.Address,
                Phone = request.Customer.Phone,
                Email = request.Customer.Email,
                CountryCode = "TR"
            },
            LineItems = request.LineItems.Select(item => new InvoiceLineItem
            {
                LineId = Guid.NewGuid(),
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.LineTotal,
                TaxRate = item.TaxRate,
                TaxAmount = item.TaxAmount,
                Unit = item.Unit,
                ProductCode = item.ProductCode
            }).ToList(),
            SubTotal = request.SubTotal,
            TaxAmount = request.TaxAmount,
            TotalAmount = request.TotalAmount,
            EshuAmount = request.EshuAmount,
            EshuDescription = request.EshuDescription,
            Session = request.Session != null ? new SessionInfo
            {
                SessionId = request.Session.SessionId,
                StationId = request.Session.StationId,
                StationName = request.Session.StationName,
                StartTime = request.Session.StartTime,
                EndTime = request.Session.EndTime,
                TotalEnergy = request.Session.TotalEnergy,
                AveragePower = request.Session.AveragePower,
                MaxPower = request.Session.MaxPower
            } : null,
            Notes = request.Notes,
            Tags = new List<string> { request.TenantId, request.ProviderKey }
        };

        return envelope;
    }

    /// <summary>
    /// IntegrationLog'a kayıt yaz
    /// </summary>
    private async Task LogIntegrationAsync(string tenantId, string operationType, string level, 
        string providerType, string message)
    {
        var log = new IntegrationLog
        {
            Level = level,
            Message = message,
            OperationType = operationType,
            ProviderType = providerType,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid().ToString()
        };

        _dbContext.IntegrationLogs.Add(log);
        await _dbContext.SaveChangesAsync();
    }
}

/// <summary>
/// Şarj oturumu fatura isteği
/// </summary>
public class ChargeSessionRequest
{
    /// <summary>
    /// Tenant ID
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Provider anahtarı
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;

    /// <summary>
    /// İdempotency anahtarı
    /// </summary>
    public string IdempotencyKey { get; set; } = string.Empty;

    /// <summary>
    /// Müşteri bilgileri
    /// </summary>
    public CustomerInfo Customer { get; set; } = new();

    /// <summary>
    /// Şarj oturumu bilgileri
    /// </summary>
    public SessionInfo? Session { get; set; }

    /// <summary>
    /// Satır kalemleri
    /// </summary>
    public List<InvoiceLineItem> LineItems { get; set; } = new();

    /// <summary>
    /// Ara toplam (KDV hariç)
    /// </summary>
    public decimal SubTotal { get; set; }

    /// <summary>
    /// KDV tutarı
    /// </summary>
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Genel toplam (KDV dahil)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// EŞÜ tutarı
    /// </summary>
    public decimal EshuAmount { get; set; }

    /// <summary>
    /// EŞÜ açıklaması
    /// </summary>
    public string? EshuDescription { get; set; }

    /// <summary>
    /// Notlar
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Toplu fatura isteği
/// </summary>
public class ChargeBatchRequest
{
    /// <summary>
    /// Rapor ID
    /// </summary>
    public Guid ReportId { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Provider anahtarı
    /// </summary>
    public string ProviderKey { get; set; } = string.Empty;
}
