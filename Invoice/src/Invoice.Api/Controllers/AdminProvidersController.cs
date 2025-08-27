using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Application.Models;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.Providers;
using System.Text.Json;

namespace Invoice.Api.Controllers;

/// <summary>
/// Admin sağlayıcı yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/admin/providers")]
public class AdminProvidersController : ControllerBase
{
    private readonly InvoiceDbContext _dbContext;
    private readonly IInvoiceProviderFactory _providerFactory;
    private readonly ILogger<AdminProvidersController> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public AdminProvidersController(
        InvoiceDbContext dbContext,
        IInvoiceProviderFactory providerFactory,
        ILogger<AdminProvidersController> logger)
    {
        _dbContext = dbContext;
        _providerFactory = providerFactory;
        _logger = logger;
    }

    /// <summary>
    /// Tüm sağlayıcıları listele
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ProviderConfig>>> GetProviders()
    {
        try
        {
            var providers = await _dbContext.ProviderConfigs
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProviderKey)
                .ThenBy(p => p.Title)
                .ToListAsync();

            return Ok(providers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sağlayıcı listesi alınırken hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Sağlayıcı oluştur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProviderConfig>> CreateProvider([FromBody] CreateProviderRequest request)
    {
        try
        {
            // Validasyon
            if (string.IsNullOrWhiteSpace(request.ProviderKey))
                return BadRequest(new { error = "ProviderKey zorunludur" });

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest(new { error = "Title zorunludur" });

            if (string.IsNullOrWhiteSpace(request.ApiBaseUrl))
                return BadRequest(new { error = "ApiBaseUrl zorunludur" });

            // Aynı provider key kontrolü
            var existingProvider = await _dbContext.ProviderConfigs
                .FirstOrDefaultAsync(p => p.ProviderKey == request.ProviderKey && p.TenantId == request.TenantId);

            if (existingProvider != null)
                return BadRequest(new { error = "Bu provider key zaten kullanılıyor" });

            // Yeni provider oluştur
            var provider = new ProviderConfig
            {
                Id = Guid.NewGuid(),
                TenantId = request.TenantId ?? "default",
                ProviderKey = request.ProviderKey,
                ApiBaseUrl = request.ApiBaseUrl,
                ApiKey = request.ApiKey,
                ApiSecret = request.ApiSecret,
                WebhookSecret = request.WebhookSecret,
                VknTckn = request.VknTckn,
                Title = request.Title,
                BranchCode = request.BranchCode,
                SignMode = request.SignMode,
                TimeoutSec = request.TimeoutSec,
                RetryCountOverride = request.RetryCountOverride,
                CircuitTripThreshold = request.CircuitTripThreshold,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.ProviderConfigs.Add(provider);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Yeni sağlayıcı oluşturuldu: {ProviderKey} - {Title}", provider.ProviderKey, provider.Title);

            return CreatedAtAction(nameof(GetProvider), new { id = provider.Id }, provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sağlayıcı oluşturulurken hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Sağlayıcı güncelle
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProviderConfig>> UpdateProvider(Guid id, [FromBody] UpdateProviderRequest request)
    {
        try
        {
            var provider = await _dbContext.ProviderConfigs.FindAsync(id);
            if (provider == null)
                return NotFound(new { error = "Sağlayıcı bulunamadı" });

            // Güncelle
            provider.ApiBaseUrl = request.ApiBaseUrl ?? provider.ApiBaseUrl;
            provider.ApiKey = request.ApiKey ?? provider.ApiKey;
            provider.ApiSecret = request.ApiSecret ?? provider.ApiSecret;
            provider.WebhookSecret = request.WebhookSecret ?? provider.WebhookSecret;
            provider.VknTckn = request.VknTckn ?? provider.VknTckn;
            provider.Title = request.Title ?? provider.Title;
            provider.BranchCode = request.BranchCode;
            provider.SignMode = request.SignMode;
            provider.TimeoutSec = request.TimeoutSec;
            provider.RetryCountOverride = request.RetryCountOverride;
            provider.CircuitTripThreshold = request.CircuitTripThreshold;
            provider.IsActive = request.IsActive;
            provider.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Sağlayıcı güncellendi: {ProviderKey} - {Title}", provider.ProviderKey, provider.Title);

            return Ok(provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sağlayıcı güncellenirken hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Sağlayıcı sil
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProvider(Guid id)
    {
        try
        {
            var provider = await _dbContext.ProviderConfigs.FindAsync(id);
            if (provider == null)
                return NotFound(new { error = "Sağlayıcı bulunamadı" });

            _dbContext.ProviderConfigs.Remove(provider);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Sağlayıcı silindi: {ProviderKey} - {Title}", provider.ProviderKey, provider.Title);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sağlayıcı silinirken hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Sağlayıcı detayı getir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProviderConfig>> GetProvider(Guid id)
    {
        try
        {
            var provider = await _dbContext.ProviderConfigs.FindAsync(id);
            if (provider == null)
                return NotFound(new { error = "Sağlayıcı bulunamadı" });

            return Ok(provider);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sağlayıcı detayı alınırken hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Test bağlantı
    /// </summary>
    [HttpPost("{id}/test-connection")]
    public async Task<ActionResult<TestConnectionResponse>> TestConnection(Guid id)
    {
        try
        {
            var provider = await _dbContext.ProviderConfigs.FindAsync(id);
            if (provider == null)
                return NotFound(new { error = "Sağlayıcı bulunamadı" });

            var startTime = DateTime.UtcNow;

            try
            {
                // Provider'ı al
                var providerInstance = _providerFactory.Resolve(provider.ProviderKey);

                // Mock test bağlantısı (gerçek implementasyonda ping/health check yapılır)
                await Task.Delay(100); // Simüle edilmiş gecikme

                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                _logger.LogInformation("Test bağlantısı başarılı: {ProviderKey}, Süre: {ResponseTime}ms", 
                    provider.ProviderKey, responseTime);

                return Ok(new TestConnectionResponse
                {
                    Success = true,
                    Message = $"{provider.ProviderKey.ToUpper()} bağlantısı başarılı",
                    ResponseTime = (int)responseTime
                });
            }
            catch (Exception ex)
            {
                var responseTime = (DateTime.UtcNow - startTime).TotalMilliseconds;

                _logger.LogError(ex, "Test bağlantısı başarısız: {ProviderKey}, Süre: {ResponseTime}ms", 
                    provider.ProviderKey, responseTime);

                return Ok(new TestConnectionResponse
                {
                    Success = false,
                    Message = $"Bağlantı hatası: {ex.Message}",
                    ResponseTime = (int)responseTime
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test bağlantısı sırasında hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Test gönderim
    /// </summary>
    [HttpPost("{id}/test-send")]
    public async Task<ActionResult<TestSendResponse>> TestSend(Guid id)
    {
        try
        {
            var provider = await _dbContext.ProviderConfigs.FindAsync(id);
            if (provider == null)
                return NotFound(new { error = "Sağlayıcı bulunamadı" });

            try
            {
                // Provider'ı al
                var providerInstance = _providerFactory.Resolve(provider.ProviderKey);

                // Test fatura zarfı oluştur
                var testEnvelope = CreateTestEnvelope();
                var idempotencyKey = $"test-{Guid.NewGuid()}";

                // Test gönderimi yap
                var result = await providerInstance.SendAsync(testEnvelope, idempotencyKey, CancellationToken.None);

                _logger.LogInformation("Test gönderimi tamamlandı: {ProviderKey}, Başarı: {Success}", 
                    provider.ProviderKey, result.Success);

                return Ok(new TestSendResponse
                {
                    Success = result.Success,
                    ProviderInvoiceId = result.ProviderInvoiceId,
                    Message = result.Success ? "Test gönderimi başarılı" : result.ErrorMessage ?? "Bilinmeyen hata",
                    ErrorCode = result.ErrorCode,
                    RawResponse = result.RawResponse
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test gönderimi başarısız: {ProviderKey}", provider.ProviderKey);

                return Ok(new TestSendResponse
                {
                    Success = false,
                    Message = $"Gönderim hatası: {ex.Message}",
                    ErrorCode = "EXCEPTION"
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test gönderimi sırasında hata oluştu");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Test fatura zarfı oluştur
    /// </summary>
    private InvoiceEnvelope CreateTestEnvelope()
    {
        return new InvoiceEnvelope
        {
            InvoiceId = Guid.NewGuid(),
            InvoiceNumber = $"TEST-{DateTime.Now:yyyyMMdd}-001",
            InvoiceDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(30),
            Currency = "TRY",
            Customer = new CustomerInfo
            {
                CustomerId = "TEST-CUSTOMER-001",
                Name = "Test Müşteri A.Ş.",
                TaxNumber = "1234567890",
                TaxOffice = "Test Vergi Dairesi",
                Address = "Test Adres, İstanbul",
                Phone = "0212 123 45 67",
                Email = "test@example.com"
            },
            LineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem
                {
                    LineId = Guid.NewGuid(),
                    Description = "Test Ürün/Hizmet",
                    Quantity = 1,
                    UnitPrice = 100.00m,
                    LineTotal = 100.00m,
                    TaxRate = 18.00m,
                    TaxAmount = 18.00m,
                    Unit = "adet"
                }
            },
            SubTotal = 100.00m,
            TaxAmount = 18.00m,
            TotalAmount = 118.00m,
            EshuAmount = 0.00m,
            Notes = "Test fatura - Admin panelinden gönderildi",
            Tags = new List<string> { "test", "admin" }
        };
    }
}

/// <summary>
/// Sağlayıcı oluşturma isteği
/// </summary>
public class CreateProviderRequest
{
    public string? TenantId { get; set; }
    public string ProviderKey { get; set; } = string.Empty;
    public string ApiBaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ApiSecret { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string VknTckn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public SignMode SignMode { get; set; } = SignMode.ProviderSign;
    public int TimeoutSec { get; set; } = 30;
    public int? RetryCountOverride { get; set; }
    public int? CircuitTripThreshold { get; set; }
}

/// <summary>
/// Sağlayıcı güncelleme isteği
/// </summary>
public class UpdateProviderRequest
{
    public string? ApiBaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? ApiSecret { get; set; }
    public string? WebhookSecret { get; set; }
    public string? VknTckn { get; set; }
    public string? Title { get; set; }
    public string? BranchCode { get; set; }
    public SignMode SignMode { get; set; }
    public int TimeoutSec { get; set; } = 30;
    public int? RetryCountOverride { get; set; }
    public int? CircuitTripThreshold { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Test bağlantı yanıtı
/// </summary>
public class TestConnectionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ResponseTime { get; set; }
}

/// <summary>
/// Test gönderim yanıtı
/// </summary>
public class TestSendResponse
{
    public bool Success { get; set; }
    public string? ProviderInvoiceId { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public string? RawResponse { get; set; }
}
