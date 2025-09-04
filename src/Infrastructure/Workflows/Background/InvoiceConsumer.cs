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

                    // Türkçe: Inbox idempotensi
                    var existed = await db.Set<InboxMessage>().FirstOrDefaultAsync(x => x.MessageId == messageId);
                    if (existed != null && existed.ProcessedAtUtc != null) return;

                    var inbox = existed ?? new InboxMessage { MessageId = messageId, Type = routingKey, Payload = body, ReceivedAtUtc = DateTime.UtcNow };
                    if (existed == null) db.Set<InboxMessage>().Add(inbox);
                    await db.SaveChangesAsync();

                    // Türkçe: Basit mesaj yönlendirme — Accepted/Rejected güncelle
                    var json = JsonSerializer.Deserialize<JsonElement>(body);
                    var wfId = json.GetProperty("Id").GetGuid();
                    var newStatus = routingKey switch
                    {
                        "Invoice.Accepted" => "Accepted",
                        "Invoice.Rejected" => "Rejected",
                        _ => "Sent"
                    };

                    var wf = await db.Set<InvoiceWorkflow>().FirstOrDefaultAsync(x => x.Id == wfId);
                    if (wf != null)
                    {
                        wf.Status = newStatus;
                        wf.LastUpdatedUtc = DateTime.UtcNow;
                        await db.SaveChangesAsync();
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
