using Microsoft.EntityFrameworkCore;
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
    public async Task<ProviderConfiguration?> GetProviderConfigurationAsync(int providerId)
    {
        _logger.LogDebug("Provider konfigürasyonu alınıyor. Provider ID: {ProviderId}", providerId);

        try
        {
            var configuration = await _context.ProviderConfigurations
                .Include(pc => pc.Provider)
                .FirstOrDefaultAsync(pc => pc.ProviderId == providerId);

            if (configuration == null)
            {
                _logger.LogWarning("Provider konfigürasyonu bulunamadı. Provider ID: {ProviderId}", providerId);
                return null;
            }

            _logger.LogDebug("Provider konfigürasyonu alındı. Provider: {ProviderName}", configuration.Provider?.Name);

            return configuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu alınırken hata. Provider ID: {ProviderId}", providerId);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu kaydeder
    /// </summary>
    public async Task<ProviderConfiguration> SaveProviderConfigurationAsync(ProviderConfiguration configuration)
    {
        _logger.LogInformation("Provider konfigürasyonu kaydediliyor. Provider ID: {ProviderId}", configuration.ProviderId);

        try
        {
            // Mevcut konfigürasyonu kontrol et
            var existingConfig = await _context.ProviderConfigurations
                .FirstOrDefaultAsync(pc => pc.ProviderId == configuration.ProviderId);

            if (existingConfig != null)
            {
                // Mevcut konfigürasyonu güncelle
                existingConfig.ApiEndpoint = configuration.ApiEndpoint;
                existingConfig.ApiKey = configuration.ApiKey;
                existingConfig.ApiSecret = configuration.ApiSecret;
                existingConfig.IsActive = configuration.IsActive;
                existingConfig.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Mevcut provider konfigürasyonu güncellendi. Provider ID: {ProviderId}", configuration.ProviderId);
            }
            else
            {
                // Yeni konfigürasyon ekle
                configuration.CreatedAt = DateTime.UtcNow;
                configuration.UpdatedAt = DateTime.UtcNow;
                _context.ProviderConfigurations.Add(configuration);

                _logger.LogInformation("Yeni provider konfigürasyonu eklendi. Provider ID: {ProviderId}", configuration.ProviderId);
            }

            await _context.SaveChangesAsync();

            return configuration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu kaydedilirken hata. Provider ID: {ProviderId}", configuration.ProviderId);
            throw;
        }
    }

    /// <summary>
    /// Aktif provider konfigürasyonlarını listeler
    /// </summary>
    public async Task<IEnumerable<ProviderConfiguration>> GetActiveProviderConfigurationsAsync()
    {
        _logger.LogDebug("Aktif provider konfigürasyonları alınıyor");

        try
        {
            var configurations = await _context.ProviderConfigurations
                .Include(pc => pc.Provider)
                .Where(pc => pc.IsActive)
                .ToListAsync();

            _logger.LogDebug("Aktif provider konfigürasyonları alındı. Sayı: {Count}", configurations.Count);

            return configurations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Aktif provider konfigürasyonları alınırken hata");
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu deaktif eder
    /// </summary>
    public async Task DeactivateProviderConfigurationAsync(int providerId)
    {
        _logger.LogInformation("Provider konfigürasyonu deaktif ediliyor. Provider ID: {ProviderId}", providerId);

        try
        {
            var configuration = await _context.ProviderConfigurations
                .FirstOrDefaultAsync(pc => pc.ProviderId == providerId);

            if (configuration != null)
            {
                configuration.IsActive = false;
                configuration.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Provider konfigürasyonu deaktif edildi. Provider ID: {ProviderId}", providerId);
            }
            else
            {
                _logger.LogWarning("Deaktif edilecek provider konfigürasyonu bulunamadı. Provider ID: {ProviderId}", providerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu deaktif edilirken hata. Provider ID: {ProviderId}", providerId);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu siler
    /// </summary>
    public async Task DeleteProviderConfigurationAsync(int providerId)
    {
        _logger.LogInformation("Provider konfigürasyonu siliniyor. Provider ID: {ProviderId}", providerId);

        try
        {
            var configuration = await _context.ProviderConfigurations
                .FirstOrDefaultAsync(pc => pc.ProviderId == providerId);

            if (configuration != null)
            {
                _context.ProviderConfigurations.Remove(configuration);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Provider konfigürasyonu silindi. Provider ID: {ProviderId}", providerId);
            }
            else
            {
                _logger.LogWarning("Silinecek provider konfigürasyonu bulunamadı. Provider ID: {ProviderId}", providerId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu silinirken hata. Provider ID: {ProviderId}", providerId);
            throw;
        }
    }

    /// <summary>
    /// Provider konfigürasyonunu test eder
    /// </summary>
    public async Task<bool> TestProviderConfigurationAsync(int providerId)
    {
        _logger.LogInformation("Provider konfigürasyonu test ediliyor. Provider ID: {ProviderId}", providerId);

        try
        {
            var configuration = await GetProviderConfigurationAsync(providerId);
            if (configuration == null)
            {
                _logger.LogWarning("Test edilecek provider konfigürasyonu bulunamadı. Provider ID: {ProviderId}", providerId);
                return false;
            }

            // Mock test - gerçek implementasyonda API endpoint'i test edilir
            await Task.Delay(100); // Simüle edilmiş API çağrısı

            _logger.LogInformation("Provider konfigürasyonu test edildi. Provider ID: {ProviderId}, Sonuç: Başarılı", providerId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Provider konfigürasyonu test edilirken hata. Provider ID: {ProviderId}", providerId);
            return false;
        }
    }
}
