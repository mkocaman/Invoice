using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Veritabanı seed işlemlerini gerçekleştiren servis
/// </summary>
public class SeedService
{
    private readonly InvoiceDbContext _context;
    private readonly ILogger<SeedService> _logger;

    public SeedService(InvoiceDbContext context, ILogger<SeedService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Tüm seed işlemlerini gerçekleştirir
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("Seed işlemi başlatılıyor...");

        try
        {
            // EŞÜ seed
            await SeedEshuAsync();

            // Örnek şarj oturumu seed
            await SeedChargeSessionAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("Seed işlemi başarıyla tamamlandı");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Seed işlemi sırasında hata oluştu");
            throw;
        }
    }

    /// <summary>
    /// Örnek EŞÜ kaydı oluşturur
    /// </summary>
    private async Task SeedEshuAsync()
    {
        var existingEshu = await _context.Eshus
            .FirstOrDefaultAsync(e => e.SerialNumber == "TEST-ESHU-001");

        if (existingEshu == null)
        {
            _logger.LogInformation("Örnek EŞÜ kaydı oluşturuluyor...");
            
            var eshu = new Eshu
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                SerialNumber = "TEST-ESHU-001",
                Model = "Test Model",
                Brand = "Test Brand",
                Type = "AC",
                Power = 22.0m,
                Voltage = 400,
                Current = 32.0m,
                Frequency = 50,
                Manufacturer = "Test Manufacturer",
                ManufacturingDate = DateTime.UtcNow.AddYears(-1),
                InstallationDate = DateTime.UtcNow.AddMonths(-6),
                Location = "Test Adres, İstanbul, Türkiye",
                Latitude = 41.0082m,
                Longitude = 28.9784m
            };

            _context.Eshus.Add(eshu);
            _logger.LogInformation("Örnek EŞÜ kaydı oluşturuldu");
        }
    }

    /// <summary>
    /// Örnek şarj oturumu oluşturur
    /// </summary>
    private async Task SeedChargeSessionAsync()
    {
        var existingSession = await _context.ChargeSessions
            .FirstOrDefaultAsync(e => e.Notes == "Test şarj oturumu");

        if (existingSession == null)
        {
            _logger.LogInformation("Örnek şarj oturumu oluşturuluyor...");
            
            var chargeSession = new ChargeSession
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                EshuId = Guid.NewGuid(), // EŞÜ ID'si
                StartDate = DateTime.UtcNow.AddHours(-2),
                EndDate = DateTime.UtcNow.AddHours(-1),
                DurationMinutes = 60,
                EnergyConsumed = 45.5m,
                ChargeAmount = 113.75m,
                ChargePower = 22.0m,
                ChargeCurrent = 32.0m,
                ChargeVoltage = 400,
                ChargeFrequency = 50,
                ChargeTemperature = 25.0m,
                Status = "COMPLETED",
                Notes = "Test şarj oturumu - ACME A.Ş.",
                IsCompleted = true,
                IsCancelled = false
            };

            _context.ChargeSessions.Add(chargeSession);
            _logger.LogInformation("Örnek şarj oturumu oluşturuldu");
        }
    }
}
