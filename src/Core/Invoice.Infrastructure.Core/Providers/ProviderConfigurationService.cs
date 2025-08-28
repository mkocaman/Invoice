using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Data;

namespace Invoice.Infrastructure.Providers;

/// <summary>
/// Provider konfigürasyon servisi
/// </summary>
public class ProviderConfigurationService : IProviderConfigurationService
{
    private readonly ILogger<ProviderConfigurationService> _logger;
    private readonly InvoiceDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProviderConfigurationService(ILogger<ProviderConfigurationService> logger, InvoiceDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Provider konfigürasyonunu alır
    /// </summary>
    public async Task<ProviderConfig?> GetAsync(string tenantId, string providerKey)
    {
        _logger.LogDebug("Provider konfigürasyonu alınıyor. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);

        try
        {
            var configuration = await _context.ProviderConfigs
                .FirstOrDefaultAsync(pc => pc.TenantId == tenantId && pc.ProviderKey == providerKey);

            if (configuration == null)
            {
                _logger.LogWarning("Provider konfigürasyonu bulunamadı. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);
                return null;
            }

            _logger.LogDebug("Provider konfigürasyonu alındı. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);

            return configuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu alınırken hata. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu oluşturur
    /// </summary>
    public async Task<ProviderConfig> CreateAsync(ProviderConfig configuration)
    {
        _logger.LogInformation("Provider konfigürasyonu kaydediliyor. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);

        try
        {
            // Mevcut konfigürasyonu kontrol et
            var existingConfig = await _context.ProviderConfigs
                .FirstOrDefaultAsync(pc => pc.TenantId == configuration.TenantId && pc.ProviderKey == configuration.ProviderKey);

            if (existingConfig != null)
            {
                // Mevcut konfigürasyonu güncelle
                existingConfig.ApiBaseUrl = configuration.ApiBaseUrl;
                existingConfig.ApiKey = configuration.ApiKey;
                existingConfig.ApiSecret = configuration.ApiSecret;
                existingConfig.WebhookSecret = configuration.WebhookSecret;
                existingConfig.VknTckn = configuration.VknTckn;
                existingConfig.Title = configuration.Title;
                existingConfig.BranchCode = configuration.BranchCode;
                existingConfig.SignMode = configuration.SignMode;
                existingConfig.TimeoutSec = configuration.TimeoutSec;
                existingConfig.RetryCountOverride = configuration.RetryCountOverride;
                existingConfig.CircuitTripThreshold = configuration.CircuitTripThreshold;
                existingConfig.IsActive = configuration.IsActive;
                existingConfig.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Mevcut provider konfigürasyonu güncellendi. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);
            }
            else
            {
                // Yeni konfigürasyon ekle
                configuration.CreatedAt = DateTime.UtcNow;
                configuration.UpdatedAt = DateTime.UtcNow;
                _context.ProviderConfigs.Add(configuration);

                _logger.LogInformation("Yeni provider konfigürasyonu eklendi. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);
            }

            await _context.SaveChangesAsync();

            return configuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu kaydedilirken hata. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu günceller
    /// </summary>
    public async Task<ProviderConfig> UpdateAsync(ProviderConfig configuration)
    {
        _logger.LogInformation("Provider konfigürasyonu güncelleniyor. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);

        try
        {
            var existingConfig = await _context.ProviderConfigs
                .FirstOrDefaultAsync(pc => pc.TenantId == configuration.TenantId && pc.ProviderKey == configuration.ProviderKey);

            if (existingConfig == null)
            {
                throw new InvalidOperationException($"Provider konfigürasyonu bulunamadı. Tenant: {configuration.TenantId}, Provider: {configuration.ProviderKey}");
            }

            // Mevcut konfigürasyonu güncelle
            existingConfig.ApiBaseUrl = configuration.ApiBaseUrl;
            existingConfig.ApiKey = configuration.ApiKey;
            existingConfig.ApiSecret = configuration.ApiSecret;
            existingConfig.WebhookSecret = configuration.WebhookSecret;
            existingConfig.VknTckn = configuration.VknTckn;
            existingConfig.Title = configuration.Title;
            existingConfig.BranchCode = configuration.BranchCode;
            existingConfig.SignMode = configuration.SignMode;
            existingConfig.TimeoutSec = configuration.TimeoutSec;
            existingConfig.RetryCountOverride = configuration.RetryCountOverride;
            existingConfig.CircuitTripThreshold = configuration.CircuitTripThreshold;
            existingConfig.IsActive = configuration.IsActive;
            existingConfig.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Provider konfigürasyonu güncellendi. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);

            return existingConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu güncellenirken hata. Tenant: {TenantId}, Provider: {ProviderKey}", configuration.TenantId, configuration.ProviderKey);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu siler
    /// </summary>
    public async Task DeleteAsync(string tenantId, string providerKey)
    {
        _logger.LogInformation("Provider konfigürasyonu siliniyor. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);

        try
        {
            var configuration = await _context.ProviderConfigs
                .FirstOrDefaultAsync(pc => pc.TenantId == tenantId && pc.ProviderKey == providerKey);

            if (configuration != null)
            {
                _context.ProviderConfigs.Remove(configuration);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Provider konfigürasyonu silindi. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);
            }
            else
            {
                _logger.LogWarning("Silinecek provider konfigürasyonu bulunamadı. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu silinirken hata. Tenant: {TenantId}, Provider: {ProviderKey}", tenantId, providerKey);
            throw;
        }
    }

    /// <summary>
    /// Tenant'a ait tüm konfigürasyonları listeler
    /// </summary>
    public async Task<IEnumerable<ProviderConfig>> ListAsync(string tenantId)
    {
        _logger.LogDebug("Aktif provider konfigürasyonları alınıyor. Tenant: {TenantId}", tenantId);

        try
        {
            var configurations = await _context.ProviderConfigs
                .Where(pc => pc.TenantId == tenantId && pc.IsActive)
                .ToListAsync();

            _logger.LogDebug("Aktif provider konfigürasyonları alındı. Tenant: {TenantId}, Sayı: {Count}", tenantId, configurations.Count);

            return configurations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif provider konfigürasyonları alınırken hata. Tenant: {TenantId}", tenantId);
            throw;
        }
    }


}
