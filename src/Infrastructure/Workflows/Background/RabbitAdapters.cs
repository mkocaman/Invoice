// Türkçe: Mevcut RabbitMqClient'ı kullanarak basit yayın/tüketim adaptörleri.
// Burayı projenizdeki gerçek RabbitMqClient API'sına uyarlayın.
using Microsoft.Extensions.Logging;

namespace Infrastructure.Workflows.Background
{
    public sealed class RabbitPublisherAdapter : IRabbitPublisher
    {
        private readonly ILogger<RabbitPublisherAdapter> _logger;
        public RabbitPublisherAdapter(ILogger<RabbitPublisherAdapter> logger) => _logger = logger;

        public Task PublishAsync(string exchange, string routingKey, string body, CancellationToken ct)
        {
            // [SIMULATION] Türkçe: Burayı gerçek publish ile değiştirin.
            _logger.LogInformation("MQ Publish → ex={Exchange}, rk={RK}, len={Len}", exchange, routingKey, body?.Length ?? 0);
            return Task.CompletedTask;
        }
    }

    public sealed class MessageConsumerAdapter : IMessageConsumer
    {
        // Türkçe: Gerçek tüketim kütüphanenize bağlayın ve mesaj geldiğinde OnMessage'i çağırın.
        public event Func<string, string, string, Task>? OnMessage;
        // [SIMULATION] Türkçe: Demo için anlık tetikleyici bırakıyoruz.
        public async Task SimulateIncomingAsync(string messageId, string routingKey, string body)
            => await (OnMessage?.Invoke(messageId, routingKey, body) ?? Task.CompletedTask);
    }
}
