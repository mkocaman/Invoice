// Türkçe Açıklama:
// Audit kayıtlarını kolay eklemek için servis. Büyük gövdeleri eklemeden önce
// hassas alanları MASKELEME sorumluluğu bu servise aittir.

using System.Security.Cryptography;
using System.Text;
using Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Db.Services;

public interface IInvoiceAuditService
{
    Task<long> LogAsync(InvoiceAudit entry, CancellationToken ct = default);
    Task<long> LogWithStatusAsync(
        InvoiceAudit entry,
        string invoiceId,
        string eventType,
        string? statusFrom,
        string? statusTo,
        string? systemCode,
        bool simulation,
        string? externalInvoiceNumber = null,
        string? eventKey = null,
        DateTime? occurredAtUtc = null,
        CancellationToken ct = default);
    string Sha256(string? text);
    string? Redact(string? payload);
}

public partial class InvoiceAuditService : IInvoiceAuditService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InvoiceAuditService> _logger;

    public InvoiceAuditService(AppDbContext db, ILogger<InvoiceAuditService> logger)
    {
        _db = db;
        _logger = logger;
    }

    // Türkçe: Basit SHA-256 helper
    public string Sha256(string? text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(text);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    // Türkçe: Maskeleme (örn. token, şifre, tc, iban) — gerekirse kuralları genişlet
    public string? Redact(string? payload)
    {
        if (string.IsNullOrEmpty(payload)) return payload;
        var redacted = payload
            .Replace("\"password\":\"", "\"password\":\"***", StringComparison.OrdinalIgnoreCase)
            .Replace("\"token\":\"", "\"token\":\"***", StringComparison.OrdinalIgnoreCase);
        return redacted;
    }

    public async Task<long> LogAsync(InvoiceAudit entry, CancellationToken ct = default)
    {
        // Türkçe: Büyük gövdeleri maskele ve hash'lerini hesapla
        entry.RequestBody = Redact(entry.RequestBody);
        entry.ResponseBody = Redact(entry.ResponseBody);

        entry.XmlSha256 = Sha256(entry.XmlPayload);
        entry.JsonSha256 = Sha256(entry.JsonPayload);
        entry.RequestSha256 = Sha256(entry.RequestBody);
        entry.ResponseSha256 = Sha256(entry.ResponseBody);

        _db.InvoiceAudits.Add(entry);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("InvoiceAudit kaydedildi: id={Id} invoice={InvoiceId} event={EventType} sys={System}",
            entry.Id, entry.InvoiceId, entry.EventType, entry.SystemCode);

        return entry.Id;
    }

    private async Task<long> WriteStatusAsync(
        string invoiceId,
        string eventType,
        string? statusFrom,
        string? statusTo,
        string? systemCode,
        bool simulation,
        string? externalInvoiceNumber,
        string? eventKey,
        DateTime occurredAtUtc,
        CancellationToken ct)
    {
        // Idempotency: Aynı (InvoiceId, EventKey) varsa yazma
        if (!string.IsNullOrWhiteSpace(eventKey))
        {
            var exists = await _db.InvoiceStatusHistory
                .AnyAsync(x => x.InvoiceId == invoiceId && x.EventKey == eventKey, ct);
            if (exists) return 0;
        }

        // Latency: bir önceki olay ile bu olay arasındaki fark
        var prev = await _db.InvoiceStatusHistory
            .Where(x => x.InvoiceId == invoiceId)
            .OrderByDescending(x => x.OccurredAtUtc)
            .FirstOrDefaultAsync(ct);

        long? latency = null;
        if (prev is not null)
            latency = (long)(occurredAtUtc - prev.OccurredAtUtc).TotalMilliseconds;

        var rec = new InvoiceStatusHistory
        {
            InvoiceId = invoiceId,
            ExternalInvoiceNumber = externalInvoiceNumber,
            EventType = eventType,
            StatusFrom = statusFrom,
            StatusTo = statusTo,
            SystemCode = systemCode,
            OccurredAtUtc = occurredAtUtc,
            LatencyMs = latency,
            EventKey = eventKey,
            Simulation = simulation
        };

        _db.InvoiceStatusHistory.Add(rec);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("StatusHistory eklendi: invoice={InvoiceId} event={EventType} latencyMs={Latency}",
            invoiceId, eventType, latency);

        return rec.Id;
    }

    public async Task<long> LogWithStatusAsync(
        InvoiceAudit entry,
        string invoiceId,
        string eventType,
        string? statusFrom,
        string? statusTo,
        string? systemCode,
        bool simulation,
        string? externalInvoiceNumber = null,
        string? eventKey = null,
        DateTime? occurredAtUtc = null,
        CancellationToken ct = default)
    {
        // 1) Audit (büyük gövde + hash + maskeleme)
        var auditId = await LogAsync(entry, ct);

        // 2) StatusHistory (hızlı rapor + latency + idempotency)
        var when = occurredAtUtc ?? DateTime.UtcNow;
        await WriteStatusAsync(invoiceId, eventType, statusFrom, statusTo, systemCode, simulation,
            externalInvoiceNumber, eventKey, when, ct);

        return auditId;
    }
}
