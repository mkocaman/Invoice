// Türkçe Açıklama:
// Publish/Consume noktalarında structured log. Publisher confirms + retry sayaçları.
// Dead-letter konfigürasyonundan gelen mesajlar header'lardan okunur ve log'a yazılır.

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Logging;

public class RabbitMqClient : IDisposable
{
    private readonly ILogger<RabbitMqClient> _logger;
    private readonly IConnection _conn;
    private readonly IModel _ch;

    public RabbitMqClient(ILogger<RabbitMqClient> logger, IConnectionFactory factory)
    {
        _logger = logger;

        // Bağlantı
        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();

        // Publisher confirms
        _ch.ConfirmSelect();
        _logger.LogInformation("RabbitMQ channel created. ConfirmSelect enabled.");
    }

    public void DeclareTopology(string exchange, string queue, string routingKey, bool durable = true, string? deadLetterExchange = null)
    {
        var args = new Dictionary<string, object>();
        if (!string.IsNullOrWhiteSpace(deadLetterExchange))
            args["x-dead-letter-exchange"] = deadLetterExchange;

        _ch.ExchangeDeclare(exchange, ExchangeType.Direct, durable);
        _ch.QueueDeclare(queue, durable, exclusive: false, autoDelete: false, arguments: args);
        _ch.QueueBind(queue, exchange, routingKey);

        _logger.LogInformation("Topology declared: exchange={Exchange}, queue={Queue}, routingKey={RoutingKey}, DLX={DLX}",
            exchange, queue, routingKey, deadLetterExchange);
    }

    public void Publish(string exchange, string routingKey, ReadOnlyMemory<byte> body, string? messageId = null, string? correlationId = null, IDictionary<string, object>? headers = null)
    {
        var props = _ch.CreateBasicProperties();
        props.MessageId = messageId ?? Guid.NewGuid().ToString("n");
        props.CorrelationId = correlationId;
        props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        props.ContentType = "application/json";
        props.Headers = headers;

        // Türkçe: Publish öncesi log
        _logger.LogInformation("RMQ Publish → exchange={Exchange} rk={RoutingKey} msgId={MessageId} corrId={CorrelationId} size={Size}",
            exchange, routingKey, props.MessageId, props.CorrelationId, body.Length);

        _ch.BasicPublish(exchange, routingKey, mandatory: true, basicProperties: props, body);

        // Publisher confirm
        if (!_ch.WaitForConfirms(TimeSpan.FromSeconds(5)))
        {
            _logger.LogError("RMQ Publish NACK/Timeout → msgId={MessageId} corrId={CorrelationId}", props.MessageId, props.CorrelationId);
            // Burada retry/sıra dışı kayıt eklenebilir
        }
        else
        {
            _logger.LogInformation("RMQ Publish ACK → msgId={MessageId}", props.MessageId);
        }
    }

    public void Consume(string queue, string consumerName, CancellationToken ct)
    {
        var consumer = new EventingBasicConsumer(_ch);
        consumer.Received += (s, ea) =>
        {
            var msg = Encoding.UTF8.GetString(ea.Body.Span);

            // Header'lar
            var retry = ea.BasicProperties.Headers?.TryGetValue("x-retry-count", out var rc) == true ? Convert.ToInt32(rc) : 0;
            var dlx = ea.Exchange;

            _logger.LogInformation("RMQ Consume ← queue={Queue} consumer={Consumer} deliveryTag={Tag} msgId={MessageId} corrId={CorrelationId} retry={Retry} dlx={DLX}",
                queue, consumerName, ea.DeliveryTag, ea.BasicProperties.MessageId, ea.BasicProperties.CorrelationId, retry, dlx);

            try
            {
                // TODO: İş kuralları burada işlenir
                // Türkçe: Başarılı ise ACK
                _ch.BasicAck(ea.DeliveryTag, multiple: false);
                _logger.LogInformation("RMQ Ack → deliveryTag={Tag}", ea.DeliveryTag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RMQ Error → Nack deliveryTag={Tag}", ea.DeliveryTag);
                _ch.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
            }
        };

        _ch.BasicQos(0, 10, global: false);
        var tag = _ch.BasicConsume(queue, autoAck: false, consumer: consumer, consumerTag: consumerName);

        _logger.LogInformation("RMQ Consumer started: queue={Queue} tag={Tag}", queue, tag);

        // Türkçe: İptal edilene kadar bekle
        ct.Register(() =>
        {
            try
            {
                _ch.BasicCancel(tag);
                _logger.LogInformation("RMQ Consumer cancelled: tag={Tag}", tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RMQ cancel error");
            }
        });
    }

    public void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
