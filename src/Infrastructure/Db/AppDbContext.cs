// Türkçe Açıklama:
// Temel AppDbContext sınıfı. Invoice modülü için migration ve DB işlemleri burada yapılır.
// Şimdilik örnek "Invoices" tablosu ekledik.

using Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Db;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Örnek entity set
    public DbSet<Invoice> Invoices => Set<Invoice>();

    // ✅ Audit kayıtları
    public DbSet<InvoiceAudit> InvoiceAudits => Set<InvoiceAudit>();

    // ✅ Durum geçmişi
    public DbSet<InvoiceStatusHistory> InvoiceStatusHistory => Set<InvoiceStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Türkçe: Audit indeksleri - sorgu performansı için
        modelBuilder.Entity<InvoiceAudit>()
            .HasIndex(x => x.InvoiceId);
        modelBuilder.Entity<InvoiceAudit>()
            .HasIndex(x => new { x.EventType, x.SystemCode });
        modelBuilder.Entity<InvoiceAudit>()
            .HasIndex(x => x.CreatedAtUtc);
        modelBuilder.Entity<InvoiceAudit>()
            .HasIndex(x => x.Simulation);

        // StatusHistory indeksleri
        modelBuilder.Entity<InvoiceStatusHistory>()
            .HasIndex(x => x.InvoiceId);
        modelBuilder.Entity<InvoiceStatusHistory>()
            .HasIndex(x => new { x.EventType, x.SystemCode });
        modelBuilder.Entity<InvoiceStatusHistory>()
            .HasIndex(x => x.OccurredAtUtc);

        // Idempotency: Aynı InvoiceId + EventKey bir kez yazılsın
        modelBuilder.Entity<InvoiceStatusHistory>()
            .HasIndex(x => new { x.InvoiceId, x.EventKey })
            .IsUnique();
    }
}

// Türkçe: Basit Invoice entity (örnek)
public class Invoice
{
    public Guid Id { get; set; }
    public string Number { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
