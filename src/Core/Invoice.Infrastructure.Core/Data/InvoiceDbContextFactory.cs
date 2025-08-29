using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Data;

/// <summary>
/// Design-time DbContext factory
/// </summary>
public class InvoiceDbContextFactory : IDesignTimeDbContextFactory<InvoiceDbContext>
{
    /// <summary>
    /// Design-time DbContext oluşturur
    /// </summary>
    /// <param name="args">Argümanlar</param>
    /// <returns>DbContext</returns>
    public InvoiceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<InvoiceDbContext>();
        
        // Development connection string - Default anahtarını kullan
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=invoice_db;Username=postgres;Password=1453");
        
        return new InvoiceDbContext(optionsBuilder.Options);
    }
}
