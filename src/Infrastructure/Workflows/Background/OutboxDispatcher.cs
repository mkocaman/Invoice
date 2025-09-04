// Türkçe: Outbox'taki bekleyen mesajları alır ve RabbitMQ'ya publish eder (güvenli gönderim + retry).
using Infrastructure.Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

// Not: Mevcut RabbitMqClient'ınızı kullanın; burada basit bir interface varsayalım:
namespace Infrastructure.Workflows.Background
{
    public interface IRabbitPublisher
    {
        Task PublishAsync(string exchange, string routingKey, string body, CancellationToken ct);
    }

    public sealed class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceProvider _sp;
        private readonly ILogger<OutboxDispatcher> _logger;

        public OutboxDispatcher(IServiceProvider sp, ILogger<OutboxDispatcher> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Türkçe: Basit periyodik tarama — prod'da timer/queue tetiklemeli yapılabilir.
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();
                    var pub = scope.ServiceProvider.GetRequiredService<IRabbitPublisher>();

                    // Türkçe: Kilitli olmayan ve gönderilmemişleri çek (limit 50)
                    var batch = await db.Set<OutboxMessage>()
                        .Where(x => x.SentAtUtc == null && (!x.Locked || x.LockedUntilUtc < DateTime.UtcNow))
                        .OrderBy(x => x.Id)
                        .Take(50)
                        .ToListAsync(stoppingToken);

                    foreach (var msg in batch)
                    {
                        // Türkçe: Basit kilit — dağıtık senaryo için SKIP LOCKED ideal
                        msg.Locked = true;
                        msg.LockedUntilUtc = DateTime.UtcNow.AddSeconds(15);
                    }
                    await db.SaveChangesAsync(stoppingToken);

                    foreach (var msg in batch)
                    {
                        try
                        {
                            await pub.PublishAsync("invoice.exchange", msg.Type, msg.Payload, stoppingToken);
                            msg.SentAtUtc = DateTime.UtcNow;
                            msg.Locked = false;
                            msg.LockedUntilUtc = null;
                            await db.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            msg.Attempt += 1;
                            msg.Locked = false;
                            msg.LockedUntilUtc = null;
                            await db.SaveChangesAsync(stoppingToken);
                            _logger.LogWarning(ex, "Outbox publish hatası Id={Id}, Attempt={Attempt}", msg.Id, msg.Attempt);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxDispatcher döngü hatası");
                }

                await Task.Delay(1500, stoppingToken);
            }
        }
    }
}
