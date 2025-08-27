using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Invoice.Infrastructure.Data;
using Invoice.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Invoice.Admin.Pages;

/// <summary>
/// Sağlayıcı ayarları sayfası model'i
/// </summary>
public class ProvidersModel : PageModel
{
    private readonly InvoiceDbContext _dbContext;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor
    /// </summary>
    public ProvidersModel(InvoiceDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    /// <summary>
    /// Sağlayıcı listesi
    /// </summary>
    public List<ProviderConfig>? Providers { get; set; }

    /// <summary>
    /// API anahtarı
    /// </summary>
    public string ApiKey => _configuration["API_KEY"] ?? "dev-api-key";

    /// <summary>
    /// Sayfa yüklendiğinde çalışır
    /// </summary>
    public async Task OnGetAsync()
    {
        // Tüm aktif sağlayıcıları getir (tenant bazlı filtreleme yapılabilir)
        Providers = await _dbContext.ProviderConfigs
            .Where(p => p.IsActive)
            .OrderBy(p => p.ProviderKey)
            .ThenBy(p => p.Title)
            .ToListAsync();
    }
}
