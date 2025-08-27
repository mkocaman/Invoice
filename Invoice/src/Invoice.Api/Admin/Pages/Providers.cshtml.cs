using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;

namespace Invoice.Api.Admin.Pages;

/// <summary>
/// Entegratör yönetimi sayfası model'i
/// </summary>
public class ProvidersModel : PageModel
{
    private readonly InvoiceDbContext _dbContext;
    private readonly ILogger<ProvidersModel> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProvidersModel(InvoiceDbContext dbContext, ILogger<ProvidersModel> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Entegratör listesi
    /// </summary>
    public List<ProviderConfig> Providers { get; set; } = new();

    /// <summary>
    /// Sayfa yüklendiğinde çalışır
    /// </summary>
    public async Task OnGetAsync()
    {
        try
        {
            _logger.LogInformation("Entegratör listesi alınıyor");

            // Aktif entegratörleri al
            Providers = await _dbContext.ProviderConfigs
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProviderKey)
                .ThenBy(p => p.Title)
                .ToListAsync();

            _logger.LogInformation("Entegratör listesi alındı. Sayı: {Count}", Providers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Entegratör listesi alınırken hata");
            // Hata durumunda boş liste döndür
            Providers = new List<ProviderConfig>();
        }
    }
}
