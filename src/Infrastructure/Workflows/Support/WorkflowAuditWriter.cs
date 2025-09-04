// Türkçe: Durum geçişlerini hem Audit hem StatusHistory tablosuna yazar
using System.Text.Json;
using Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Workflows.Support
{
    public static class WorkflowAuditWriter
    {
        // Türkçe: Basit yazıcı — null alanlar opsiyoneldir.
        public static async Task WriteAsync(DbContext db, Guid workflowId, string invoiceId,
            string from, string to, string systemCode, string providerKey,
            string? req, string? res, bool simulation, string? notes = null, long? latencyMs = null,
            CancellationToken ct = default)
        {
            // Türkçe: InvoiceAudits varsa ona da yazalım (yoksa yalnızca StatusHistory)
            var history = new Infrastructure.Db.Entities.InvoiceStatusHistory
            {
                InvoiceId = invoiceId,
                EventType = to.ToUpperInvariant(),
                StatusFrom = from,
                StatusTo = to,
                SystemCode = systemCode,
                OccurredAtUtc = DateTime.UtcNow,
                LatencyMs = (int?)(latencyMs ?? 0),
                EventKey = $"{workflowId:N}-{to}",
                Simulation = simulation
            };
            db.Set<Infrastructure.Db.Entities.InvoiceStatusHistory>().Add(history);

            // Türkçe: Audit tablosu mevcutsa payload yakala
            var audit = new Infrastructure.Db.Entities.InvoiceAudit
            {
                InvoiceId = invoiceId,
                EventType = to.ToUpperInvariant(),
                StatusFrom = from,
                StatusTo = to,
                SystemCode = systemCode,
                CorrelationId = null,
                TraceId = null,
                RequestBody = req,
                ResponseBody = res,
                Simulation = simulation,
                CreatedAtUtc = DateTime.UtcNow,
                Notes = notes
            };
            db.Set<Infrastructure.Db.Entities.InvoiceAudit>().Add(audit);

            await db.SaveChangesAsync(ct);
        }
    }
}
