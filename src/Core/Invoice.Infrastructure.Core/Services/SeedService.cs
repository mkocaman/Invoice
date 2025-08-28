using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Demo/seed veri servisi - yalnızca lokal/deneme amaçlı
/// </summary>
public class SeedService
{
    private readonly InvoiceDbContext _dbContext;
    private readonly ILogger<SeedService> _logger;

    public SeedService(InvoiceDbContext dbContext, ILogger<SeedService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Demo provider konfigürasyonu ekle
    /// </summary>
    /// <param name="tenantId">Tenant ID</param>
    /// <param name="providerKey">Provider anahtarı</param>
    /// <param name="ct">Cancellation token</param>
    public async Task SeedProviderConfigAsync(string tenantId, string providerKey, CancellationToken ct = default)
    {
        try
        {
            // Eğer zaten varsa ekleme
            var existing = await _dbContext.ProviderConfigs
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.ProviderKey == providerKey, ct);

            if (existing != null)
            {
                _logger.LogInformation("Demo provider config zaten mevcut: {TenantId}/{ProviderKey}", tenantId, providerKey);
                return;
            }

            // Yeni demo provider config oluştur
            var providerConfig = new ProviderConfig
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ProviderKey = providerKey,
                ApiBaseUrl = "https://mock.local",
                ApiKey = "demo",
                ApiSecret = "demo",
                WebhookSecret = "demo",
                VknTckn = "1234567890",
                Title = "Demo Müşteri",
                BranchCode = "MERKEZ",
                SignMode = SignMode.SelfSign,
                TimeoutSec = 30,
                RetryCountOverride = 3,
                CircuitTripThreshold = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _dbContext.ProviderConfigs.Add(providerConfig);
            await _dbContext.SaveChangesAsync(ct);

            _logger.LogInformation("Demo provider config eklendi: {TenantId}/{ProviderKey}", tenantId, providerKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Demo provider config eklenirken hata oluştu: {TenantId}/{ProviderKey}", tenantId, providerKey);
            throw;
        }
    }
}
