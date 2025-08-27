using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace SendWorker;

/// <summary>
/// Fatura gönderimi için background worker
/// invoice.toSend kuyruğunu dinler ve faturaları gönderir
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
        _logger.LogInformation("SendWorker başlatıldı - invoice.toSend kuyruğunu dinliyor");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Bekleyen faturaları işle
                await ProcessPendingInvoicesAsync();

                // 5 saniye bekle
                await Task.Delay(5000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendWorker çalışırken hata oluştu");
                await Task.Delay(10000, stoppingToken); // Hata durumunda daha uzun bekle
            }
        }
    }

    /// <summary>
    /// Gönderim bekleyen faturaları işler
    /// </summary>
    private async Task ProcessPendingInvoicesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

        // Gönderim bekleyen faturaları al
        var pendingInvoices = await context.Invoices
            .Where(f => f.Status == InvoiceStatus.READY_TO_SEND)
            .Include(f => f.InvoiceDetails)
            .Take(10) // Her seferde en fazla 10 fatura işle
            .ToListAsync();

        if (!pendingInvoices.Any())
        {
            _logger.LogDebug("Gönderim bekleyen fatura bulunamadı");
            return;
        }

        _logger.LogInformation("{Count} adet fatura işlenecek", pendingInvoices.Count);

        foreach (var invoice in pendingInvoices)
        {
            try
            {
                await ProcessInvoiceAsync(invoice, context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fatura işlenirken hata oluştu: {InvoiceId}", invoice.Id);
                
                // Fatura durumunu ERROR olarak güncelle
                invoice.Status = InvoiceStatus.ERROR;
                invoice.UpdatedAt = DateTime.UtcNow;
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Tek bir faturayı işler ve gönderir
    /// </summary>
    private async Task ProcessInvoiceAsync(Invoice.Domain.Entities.Invoice invoice, InvoiceDbContext context)
    {
        _logger.LogInformation("Fatura işleniyor: {InvoiceId}, {InvoiceNumber}", invoice.Id, invoice.InvoiceNumber);

        // Fatura durumunu SENDING olarak güncelle
        invoice.Status = InvoiceStatus.SENDING;
        invoice.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Mock provider ile fatura gönderimi simüle et
        var success = await SendInvoiceToProviderAsync(invoice);

        if (success)
        {
            // Başarılı gönderim
            invoice.Status = InvoiceStatus.SENT;
            invoice.SentAt = DateTime.UtcNow;
            _logger.LogInformation("Fatura başarıyla gönderildi: {InvoiceId}", invoice.Id);
        }
        else
        {
            // Başarısız gönderim
            invoice.Status = InvoiceStatus.ERROR;
            _logger.LogWarning("Fatura gönderimi başarısız: {InvoiceId}", invoice.Id);
        }

        invoice.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Mock provider ile fatura gönderimi simüle eder
    /// Development ortamında başarı simulasyonu yapar
    /// </summary>
    private async Task<bool> SendInvoiceToProviderAsync(Invoice.Domain.Entities.Invoice invoice)
    {
        // Simüle edilmiş gönderim süresi (1-3 saniye)
        var sendDelay = Random.Shared.Next(1000, 3000);
        await Task.Delay(sendDelay);

        // Development ortamında %95 başarı oranı
        var success = Random.Shared.Next(1, 101) <= 95;

        _logger.LogInformation("Mock provider gönderimi tamamlandı: {InvoiceId}, Başarı: {Success}, Süre: {Delay}ms", 
            invoice.Id, success, sendDelay);

        return success;
    }
}
