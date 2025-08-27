using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        
        // Development connection string - Postgres anahtarını kullan
        optionsBuilder.UseNpgsql("Host=192.168.1.250;Port=5432;Database=invoice_db;Username=postgres;Password=LC+/34:N$1#[<?N;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;Timeout=15");
        
        return new InvoiceDbContext(optionsBuilder.Options);
    }
}
