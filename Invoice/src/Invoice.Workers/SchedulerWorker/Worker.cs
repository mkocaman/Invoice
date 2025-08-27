using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace SchedulerWorker;

/// <summary>
/// Zamanlanmış görevler için background worker
/// 7-gün batch ve 15'i aylık özet işlemlerini gerçekleştirir
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Worker'ın ana çalışma döngüsü
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SchedulerWorker başlatıldı - zamanlanmış görevleri dinliyor");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;

                // 7-gün batch işlemi (her gün saat 02:00'de çalışır)
                if (now.Hour == 2 && now.Minute == 0)
                {
                    await Process7DayBatchAsync();
                }

                // 15'i aylık özet işlemi (her ayın 15'i saat 03:00'de çalışır)
                if (now.Day == 15 && now.Hour == 3 && now.Minute == 0)
                {
                    await ProcessMonthlySummaryAsync();
                }

                // 1 dakika bekle
                await Task.Delay(60000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SchedulerWorker çalışırken hata oluştu");
                await Task.Delay(300000, stoppingToken); // Hata durumunda 5 dakika bekle
            }
        }
    }

    /// <summary>
    /// 7-gün batch işlemini gerçekleştirir
    /// Son 7 günlük şarj oturumlarını toplar ve rapor oluşturur
    /// </summary>
    private async Task Process7DayBatchAsync()
    {
        _logger.LogInformation("7-gün batch işlemi başlatılıyor...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

        try
        {
            var endDate = DateTime.UtcNow.Date;
            var startDate = endDate.AddDays(-7);

            // Son 7 günlük şarj oturumlarını al
            var chargeSessions = await context.ChargeSessions
                .Where(cs => cs.CreatedAt >= startDate && cs.CreatedAt <= endDate)
                .ToListAsync();

            if (!chargeSessions.Any())
            {
                _logger.LogInformation("7-gün batch işlemi: İşlenecek şarj oturumu bulunamadı");
                return;
            }

            // Rapor ID oluştur
            var reportId = new ReportId
            {
                Id = Guid.NewGuid(),
                ReportNumber = $"RAPOR-{startDate:yyyyMMdd}-{endDate:yyyyMMdd}",
                StartDate = startDate,
                EndDate = endDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            context.ReportIds.Add(reportId);

            // Her şarj oturumu için fatura oluştur
            foreach (var session in chargeSessions)
            {
                var invoice = new Invoice.Domain.Entities.Invoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = $"INV-{session.Id:N}",
                    ChargeSessionId = session.Id,
                    ReportId = reportId.Id,
                    Status = InvoiceStatus.READY_TO_SEND,
                    Description = "Şarj oturumu faturası",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                context.Invoices.Add(invoice);
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("7-gün batch işlemi tamamlandı: {ReportId}, Seans sayısı: {Count}", 
                reportId.Id, chargeSessions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "7-gün batch işlemi sırasında hata oluştu");
        }
    }

    /// <summary>
    /// 15'i aylık özet işlemini gerçekleştirir
    /// Önceki ayın faturalarını özetler
    /// </summary>
    private async Task ProcessMonthlySummaryAsync()
    {
        _logger.LogInformation("15'i aylık özet işlemi başlatılıyor...");

        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

        try
        {
            var now = DateTime.UtcNow;
            var previousMonth = now.AddMonths(-1);
            var startDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Önceki ayın faturalarını al
            var invoices = await context.Invoices
                .Where(i => i.CreatedAt >= startDate && i.CreatedAt <= endDate)
                .ToListAsync();

            if (!invoices.Any())
            {
                _logger.LogInformation("15'i aylık özet işlemi: İşlenecek fatura bulunamadı");
                return;
            }

            // Özet istatistikleri hesapla
            var totalInvoices = invoices.Count;
            var successfulInvoices = invoices.Count(i => i.Status == InvoiceStatus.SENT);
            var failedInvoices = invoices.Count(i => i.Status == InvoiceStatus.ERROR);
            var totalAmount = invoices.Where(i => i.Status == InvoiceStatus.SENT)
                .Sum(i => i.InvoiceDetails?.Sum(d => d.UnitPrice * d.Quantity) ?? 0);

            _logger.LogInformation("15'i aylık özet tamamlandı: {Month}/{Year}, Toplam: {Total}, Başarılı: {Success}, Başarısız: {Failed}, Toplam Tutar: {Amount:C}", 
                previousMonth.Month, previousMonth.Year, totalInvoices, successfulInvoices, failedInvoices, totalAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "15'i aylık özet işlemi sırasında hata oluştu");
        }
    }

    /// <summary>
    /// Manuel tetikleme için 7-gün batch işlemi
    /// </summary>
    public async Task Trigger7DayBatchAsync()
    {
        _logger.LogInformation("7-gün batch işlemi manuel olarak tetiklendi");
        await Process7DayBatchAsync();
    }

    /// <summary>
    /// Manuel tetikleme için aylık özet işlemi
    /// </summary>
    public async Task TriggerMonthlySummaryAsync()
    {
        _logger.LogInformation("Aylık özet işlemi manuel olarak tetiklendi");
        await ProcessMonthlySummaryAsync();
    }
}
