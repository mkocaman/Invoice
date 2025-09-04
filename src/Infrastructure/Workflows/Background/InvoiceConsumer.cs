// Türkçe: RabbitMQ'dan gelen mesajları idempotent olarak işler, workflow durumlarını günceller.
using Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Workflows.Background
{
    public interface IMessageConsumer // Türkçe: MQ layer'ınız bu interface ile mesajları fırlatabilir
    {
        // Türkçe: MQ kütüphanenizle entegre edin — bu örnek polling yerine "Push" varsayar.
        event Func<string /*messageId*/, string /*routingKey*/, string /*body*/, Task> OnMessage;
    }

    public sealed class InvoiceConsumer : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<InvoiceConsumer> _logger;
        private readonly IMessageConsumer _consumer;
        private const int MaxRetry = 5; // Türkçe: Maksimum yeniden deneme
        private static readonly TimeSpan[] Backoff = new[]
        {
            TimeSpan.FromMilliseconds(200),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(5)
        };

        public InvoiceConsumer(IServiceProvider sp, ILogger<InvoiceConsumer> logger, IMessageConsumer consumer)
        {
            _sp = sp;
            _logger = logger;
            _consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Türkçe: Mesaj geldiğinde idempotent işler
            _consumer.OnMessage += async (messageId, routingKey, body) =>
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();
                    var registry = scope.ServiceProvider.GetRequiredService<Infrastructure.Providers.IProviderRegistry>();
                    var invoker = scope.ServiceProvider.GetRequiredService<Infrastructure.Providers.Selection.IProviderInvoker>();
                    var sender  = scope.ServiceProvider.GetRequiredService<Invoice.Application.Providers.IInvoiceSender>();

                    // Türkçe: Inbox idempotensi
                    var existed = await db.Set<InboxMessage>().FirstOrDefaultAsync(x => x.MessageId == messageId);
                    if (existed != null && existed.ProcessedAtUtc != null) return;

                    var inbox = existed ?? new InboxMessage { MessageId = messageId, Type = routingKey, Payload = body, ReceivedAtUtc = DateTime.UtcNow };
                    if (existed == null) db.Set<InboxMessage>().Add(inbox);
                    await db.SaveChangesAsync();

                    // Türkçe: Mesaj ayrıştırma
                    var json = JsonSerializer.Deserialize<JsonElement>(body);
                    var wfId = json.TryGetProperty("Id", out var idEl) ? idEl.GetGuid() : Guid.Empty;
                    var amount = json.TryGetProperty("amount", out var aEl) ? aEl.GetDecimal() : 0m;
                    var currency = json.TryGetProperty("currency", out var cEl) ? cEl.GetString() ?? "TRY" : "TRY";
                    var rawPayload = json.TryGetProperty("rawPayload", out var pEl) ? pEl.GetString() : null;
                    var correlationId = json.TryGetProperty("correlationId", out var corrEl) ? corrEl.GetString() : null;

                    var wf = await db.Set<InvoiceWorkflow>().FirstOrDefaultAsync(x => x.Id == wfId);
                    if (wf is null)
                        throw new InvalidOperationException("Workflow bulunamadı.");

                    var fromState = wf.Status;
                    var country = wf.CountryCode;
                    var providerKey = wf.ProviderKey ?? "";

                    // Türkçe: Eğer mesaj "Invoice.Submitted" ise — SEND hattını tetikle
                    if (routingKey == "Invoice.Submitted")
                    {
                        var primary = registry.ResolveBest(country, "eInvoice", providerKey);
                        if (primary is null)
                            throw new InvalidOperationException($"Uygun sağlayıcı yok: {country}");

                        var fallbacks = registry.GetByCountry(country).Where(p => p.Key != primary.Key);

                        // Türkçe: Gönder — retry/backoff + fallback ile
                        Exception? last = null;
                        var swOverall = System.Diagnostics.Stopwatch.StartNew();
                        foreach (var attempt in Enumerable.Range(0, MaxRetry))
                        {
                            try
                            {
                                var sw = System.Diagnostics.Stopwatch.StartNew();
                                var result = await invoker.InvokeAsync(
                                    primary,
                                    p => sender.SendAsync(p.CountryCode, p.Key, wf.InvoiceId, rawPayload, CancellationToken.None),
                                    fallbacks);
                                sw.Stop();
                                Infrastructure.Observability.SlaMetrics.ProviderSendDuration
                                    .WithLabels(primary.Key, country).Observe(sw.Elapsed.TotalSeconds);

                                if (result.ok)
                                {
                                    // Türkçe: FSM — Submitted -> Sent
                                    var to = Invoice.Application.Workflows.InvoiceStates.Sent;
                                    if (Invoice.Application.Workflows.InvoiceFsm.Can(fromState, Invoice.Application.Workflows.InvoiceEvents.Send, to))
                                    {
                                        wf.Status = to;
                                        wf.LastUpdatedUtc = DateTime.UtcNow;
                                        await db.SaveChangesAsync();
                                        Infrastructure.Observability.SlaMetrics.WorkflowTransitions
                                            .WithLabels(fromState, to, primary.Key, country).Inc();
                                    }

                                    // Türkçe: Audit/History
                                    await Infrastructure.Workflows.Support.WorkflowAuditWriter.WriteAsync(
                                        db, wf.Id, wf.InvoiceId, fromState, Invoice.Application.Workflows.InvoiceStates.Sent,
                                        country, primary.Key, rawPayload, result.rawResponse ?? "", simulation: false,
                                        notes: "Provider send ok", latencyMs: (long)sw.Elapsed.TotalMilliseconds);

                                    inbox.Succeeded = true;
                                    inbox.ProcessedAtUtc = DateTime.UtcNow;
                                    await db.SaveChangesAsync();
                                    return;
                                }

                                // Türkçe: Başarısız ama exception fırlatmadıysa retry'a düş
                                Infrastructure.Observability.SlaMetrics.ProviderErrors
                                    .WithLabels(primary.Key, country, "send-nok").Inc();
                                Infrastructure.Observability.SlaMetrics.SendRetries
                                    .WithLabels(primary.Key, country).Inc();
                            }
                            catch (Exception ex)
                            {
                                last = ex;
                                Infrastructure.Observability.SlaMetrics.ProviderErrors
                                    .WithLabels(primary.Key, country, "exception").Inc();
                                _logger.LogWarning(ex, "Provider send hatası, attempt={Attempt}", attempt + 1);
                            }

                            var wait = attempt < Backoff.Length ? Backoff[attempt] : Backoff.Last();
                            await Task.Delay(wait);
                        }
                        swOverall.Stop();

                        // Türkçe: DLQ senaryosu — tüm denemeler tükendi
                        var toErr = Invoice.Application.Workflows.InvoiceStates.Error;
                        if (Invoice.Application.Workflows.InvoiceFsm.Can(fromState, Invoice.Application.Workflows.InvoiceEvents.Fail, toErr))
                        {
                            wf.Status = toErr;
                            wf.LastUpdatedUtc = DateTime.UtcNow;
                            wf.LastError = last?.Message ?? "Send failed";
                            await db.SaveChangesAsync();
                            Infrastructure.Observability.SlaMetrics.WorkflowTransitions
                                .WithLabels(fromState, toErr, providerKey, country).Inc();
                        }

                        await Infrastructure.Workflows.Support.WorkflowAuditWriter.WriteAsync(
                            db, wf.Id, wf.InvoiceId, fromState, Invoice.Application.Workflows.InvoiceStates.Error,
                            country, providerKey, rawPayload, last?.ToString(), simulation: false,
                            notes: "Send DLQ", latencyMs: (long)swOverall.Elapsed.TotalMilliseconds);

                        inbox.Succeeded = false;
                        inbox.Error = last?.Message ?? "Unknown";
                        inbox.ProcessedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                        return;
                    }

                    // Türkçe: Kabul/red mesajları — Sent -> Accepted/Rejected
                    if (routingKey is "Invoice.Accepted" or "Invoice.Rejected")
                    {
                        var to = routingKey == "Invoice.Accepted"
                            ? Invoice.Application.Workflows.InvoiceStates.Accepted
                            : Invoice.Application.Workflows.InvoiceStates.Rejected;

                        if (Invoice.Application.Workflows.InvoiceFsm.Can(fromState, routingKey == "Invoice.Accepted" ? Invoice.Application.Workflows.InvoiceEvents.Accept : Invoice.Application.Workflows.InvoiceEvents.Reject, to))
                        {
                            wf.Status = to;
                            wf.LastUpdatedUtc = DateTime.UtcNow;
                            await db.SaveChangesAsync();
                            Infrastructure.Observability.SlaMetrics.WorkflowTransitions
                                .WithLabels(fromState, to, providerKey, country).Inc();
                        }

                        await Infrastructure.Workflows.Support.WorkflowAuditWriter.WriteAsync(
                            db, wf.Id, wf.InvoiceId, fromState, to, country, providerKey, null, body, simulation: false,
                            notes: "Provider callback");

                        inbox.Succeeded = true;
                        inbox.ProcessedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                        return;
                    }

                    inbox.Succeeded = true;
                    inbox.ProcessedAtUtc = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();
                    var inbox = db.Set<InboxMessage>().OrderByDescending(x => x.Id).FirstOrDefault();
                    if (inbox != null)
                    {
                        inbox.Succeeded = false;
                        inbox.Error = ex.Message;
                        inbox.ProcessedAtUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync();
                    }
                    _logger.LogError(ex, "InvoiceConsumer işlem hatası");
                }
            };

            return Task.CompletedTask;
        }
    }
}
