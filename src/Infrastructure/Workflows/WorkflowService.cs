// Türkçe: İş akışı servisleri — submit, status okuma, outbox'a yazma.
using System.Text.Json;
using Infrastructure.Db.Entities;
using Infrastructure.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Workflows
{
    public interface IWorkflowService
    {
        Task<(InvoiceWorkflow wf, string jsonResponse)> SubmitAsync(
            string idemKey, string country, string capability, string? preferKey,
            decimal amount, string currency, string? description, string? rawPayload,
            string correlationId, CancellationToken ct);

        Task<InvoiceWorkflow?> GetStatusAsync(Guid workflowId, CancellationToken ct);
    }

    public sealed class WorkflowService : IWorkflowService
    {
        private readonly DbContext _db;
        private readonly IProviderRegistry _registry;
        private readonly ILogger<WorkflowService> _logger;

        public WorkflowService(DbContext db, IProviderRegistry registry, ILogger<WorkflowService> logger)
        {
            _db = db;
            _registry = registry;
            _logger = logger;
        }

        public async Task<(InvoiceWorkflow wf, string jsonResponse)> SubmitAsync(
            string idemKey, string country, string capability, string? preferKey,
            decimal amount, string currency, string? description, string? rawPayload,
            string correlationId, CancellationToken ct)
        {
            // Türkçe: İdempotensi kontrolü — aynı idemKey için kayıt varsa aynı yanıtı döndür.
            var idem = await _db.Set<IdempotencyKey>()
                .FirstOrDefaultAsync(x => x.Key == idemKey && x.Scope == "Submit", ct);
            if (idem != null)
            {
                _logger.LogInformation("İdempotent yanıt döndü: {Key}", idemKey);
                var snap = JsonSerializer.Deserialize<InvoiceWorkflow>(idem.Response)!;
                return (snap, idem.Response);
            }

            // Türkçe: Sağlayıcı çözümü
            var provider = _registry.ResolveBest(country, capability, preferKey)
                           ?? throw new InvalidOperationException($"Uygun sağlayıcı yok: {country}/{capability}");

            // Türkçe: İş akışı kaydı
            var wf = new InvoiceWorkflow
            {
                Id = Guid.NewGuid(),
                InvoiceId = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000,9999)}",
                CountryCode = country,
                ProviderKey = provider.ProviderType.ToString(),
                Status = "Submitted",
                CreatedAtUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };
            _db.Set<InvoiceWorkflow>().Add(wf);

            // Türkçe: Outbox — "Invoice.Submitted" mesajını kuyruğa göndermek üzere yaz.
            var outMsg = new OutboxMessage
            {
                AggregateId = wf.Id.ToString("N"),
                Type = "Invoice.Submitted",
                Payload = JsonSerializer.Serialize(new
                {
                    wf.Id,
                    wf.InvoiceId,
                    wf.CountryCode,
                    wf.ProviderKey,
                    amount,
                    currency,
                    description,
                    rawPayload,
                    correlationId
                })
            };
            _db.Set<OutboxMessage>().Add(outMsg);
            await _db.SaveChangesAsync(ct);

            // Türkçe: İlk yanıtı idempotent snapshot olarak sakla
            var responseJson = JsonSerializer.Serialize(wf);
            _db.Set<IdempotencyKey>().Add(new IdempotencyKey
            {
                Key = idemKey,
                Scope = "Submit",
                Response = responseJson
            });
            await _db.SaveChangesAsync(ct);

            return (wf, responseJson);
        }

        public Task<InvoiceWorkflow?> GetStatusAsync(Guid workflowId, CancellationToken ct)
        {
            return _db.Set<InvoiceWorkflow>().FirstOrDefaultAsync(x => x.Id == workflowId, ct);
        }
    }
}
