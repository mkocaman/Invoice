// Türkçe: DbContext için genişletmeler — entity setleri ve model kuralları
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db.Entities;

namespace Infrastructure.Db.Repositories
{
    public static class AppDbContextExtensions
    {
        // Türkçe: Bu metodu AppDbContext.OnModelCreating içinde veya partial sınıfta çağırın.
        public static void ConfigureWorkflowModels(this ModelBuilder model)
        {
            model.Entity<InvoiceWorkflow>()
                .HasIndex(x => x.InvoiceId)
                .HasDatabaseName("IX_InvoiceWorkflows_InvoiceId");

            model.Entity<OutboxMessage>()
                .HasIndex(x => new { x.SentAtUtc, x.Locked, x.LockedUntilUtc })
                .HasDatabaseName("IX_Outbox_Scan");

            model.Entity<InboxMessage>()
                .HasIndex(x => x.MessageId)
                .IsUnique()
                .HasDatabaseName("UX_Inbox_MessageId");

            model.Entity<IdempotencyKey>()
                .HasIndex(x => new { x.Key, x.Scope })
                .IsUnique()
                .HasDatabaseName("UX_Idem_KeyScope");
        }
    }
}
