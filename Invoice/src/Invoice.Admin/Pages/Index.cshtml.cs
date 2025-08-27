using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Admin.Pages;

/// <summary>
/// Admin ana sayfa model sınıfı
/// Fatura listesi ve sistem durumu bilgilerini sağlar
/// </summary>
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly InvoiceDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, InvoiceDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Monitoring widget verileri
    public int DailySendCount { get; set; }
    public decimal SuccessRate { get; set; }
    public int P95SendTime { get; set; }
    public int PendingInvoiceCount { get; set; }

    // Sistem durumu
    public bool ApiStatus { get; set; }
    public bool DatabaseStatus { get; set; }
    public bool SendWorkerStatus { get; set; }
    public bool SchedulerWorkerStatus { get; set; }

    // Fatura listesi
    public List<InvoiceListItem> Invoices { get; set; } = new();

    /// <summary>
    /// Sayfa yüklendiğinde çalışan metod
    /// Monitoring verilerini ve fatura listesini yükler
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Admin ana sayfa yükleniyor...");

            // Monitoring verilerini yükle
            await LoadMonitoringDataAsync();

            // Sistem durumunu kontrol et
            await CheckSystemStatusAsync();

            // Fatura listesini yükle
            await LoadInvoiceListAsync();

            _logger.LogInformation("Admin ana sayfa başarıyla yüklendi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin ana sayfa yüklenirken hata oluştu");
        }
    }

    /// <summary>
    /// Monitoring widget verilerini yükler
    /// </summary>
    private async Task LoadMonitoringDataAsync()
    {
        var today = DateTime.Today;

        // Günlük gönderim sayısı
        DailySendCount = await _context.Invoices
            .Where(i => i.CreatedAt >= today && i.Status == InvoiceStatus.SENT)
            .CountAsync();

        // Son 24 saat başarı oranı
        var last24Hours = DateTime.UtcNow.AddHours(-24);
        var totalInvoices = await _context.Invoices
            .Where(i => i.CreatedAt >= last24Hours)
            .CountAsync();
        
        var successfulInvoices = await _context.Invoices
            .Where(i => i.CreatedAt >= last24Hours && i.Status == InvoiceStatus.SENT)
            .CountAsync();

        SuccessRate = totalInvoices > 0 ? (decimal)successfulInvoices / totalInvoices * 100 : 0;

        // P95 gönderim süresi (simüle edilmiş)
        P95SendTime = 250; // ms

        // Bekleyen fatura sayısı
        PendingInvoiceCount = await _context.Invoices
            .Where(i => i.Status == InvoiceStatus.READY_TO_SEND)
            .CountAsync();
    }

    /// <summary>
    /// Sistem durumunu kontrol eder
    /// </summary>
    private async Task CheckSystemStatusAsync()
    {
        try
        {
            // Veritabanı durumu
            await _context.Database.CanConnectAsync();
            DatabaseStatus = true;
        }
        catch
        {
            DatabaseStatus = false;
        }

        // Diğer servislerin durumu (simüle edilmiş)
        ApiStatus = true;
        SendWorkerStatus = true;
        SchedulerWorkerStatus = true;
    }

    /// <summary>
    /// Fatura listesini yükler
    /// </summary>
    private async Task LoadInvoiceListAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.InvoiceDetails)
            .OrderByDescending(i => i.CreatedAt)
            .Take(20)
            .Select(i => new InvoiceListItem
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                CustomerName = "Bilinmeyen Müşteri",
                TotalAmount = i.InvoiceDetails.Sum(d => d.UnitPrice * d.Quantity),
                Status = i.Status.ToString(),
                CreatedAt = i.CreatedAt
            })
            .ToListAsync();

        Invoices = invoices;
    }
}

/// <summary>
/// Fatura listesi için kullanılan model sınıfı
/// </summary>
public class InvoiceListItem
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
