using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;

namespace Invoice.Admin.Pages;

/// <summary>
/// Kuyruklar sayfası model'i
/// </summary>
public class QueuesModel : PageModel
{
    private readonly InvoiceDbContext _context;
    private readonly ILogger<QueuesModel> _logger;

    /// <summary>
    /// Retry işleri listesi
    /// </summary>
    public List<RetryJob> RetryJobs { get; set; } = new();

    /// <summary>
    /// DLQ mesajları listesi
    /// </summary>
    public List<OutboxMessage> DlqMessages { get; set; } = new();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context">DbContext</param>
    /// <param name="logger">Logger</param>
    public QueuesModel(InvoiceDbContext context, ILogger<QueuesModel> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Sayfa yüklendiğinde çalışır
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            // Retry işlerini al (son 200 kayıt)
            RetryJobs = await _context.RetryJobs
                .Where(j => j.Status == "Pending" || j.Status == "Failed")
                .OrderByDescending(j => j.CreatedAt)
                .Take(200)
                .ToListAsync();

            // DLQ mesajlarını al (Failed durumundaki son 200 kayıt)
            DlqMessages = await _context.OutboxMessages
                .Where(m => m.Status == "Failed")
                .OrderByDescending(m => m.CreatedAt)
                .Take(200)
                .ToListAsync();

            _logger.LogInformation("Kuyruklar sayfası yüklendi. Retry: {RetryCount}, DLQ: {DlqCount}", 
                RetryJobs.Count, DlqMessages.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kuyruklar sayfası yüklenirken hata oluştu");
        }
    }
}
