// Türkçe: Audit ve StatusHistory tablolarından SLA raporları üretir
using Microsoft.EntityFrameworkCore;
using Infrastructure.Db;

namespace Infrastructure.Db.Services;

public class SLAReportService
{
    private readonly AppDbContext _db;

    public SLAReportService(AppDbContext db) => _db = db;

    // Türkçe: Ortalama gönderim → kabul süresi (ms)
    public async Task<double> GetAvgLatencyMsAsync(string systemCode)
    {
        var query = await _db.InvoiceStatusHistory
            .Where(x => x.SystemCode == systemCode && x.StatusFrom == "SENT" && x.StatusTo == "ACCEPTED")
            .Select(x => x.LatencyMs)
            .ToListAsync();

        return query.Count == 0 ? 0 : query.Average() ?? 0;
    }

    // Türkçe: Son 24 saatte hata oranı
    public async Task<double> GetErrorRateAsync(string systemCode)
    {
        var total = await _db.InvoiceStatusHistory.CountAsync(x => x.SystemCode == systemCode && x.OccurredAtUtc > DateTime.UtcNow.AddDays(-1));
        var errors = await _db.InvoiceStatusHistory.CountAsync(x => x.SystemCode == systemCode && x.EventType == "ERROR" && x.OccurredAtUtc > DateTime.UtcNow.AddDays(-1));

        return total == 0 ? 0 : (double)errors / total;
    }
}
