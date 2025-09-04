// Türkçe Açıklama:
// Test controller - RabbitMQ ve DB loglarını tetiklemek için

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IConnectionFactory _connectionFactory;

    public TestController(ILogger<TestController> logger, IConnectionFactory connectionFactory)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    [AllowAnonymous]
    [HttpPost("rabbitmq")]
    public IActionResult TestRabbitMq()
    {
        _logger.LogInformation("RabbitMQ test başlatılıyor");

        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();

            // Test mesajı gönder
            var message = "Test mesajı - " + DateTime.UtcNow;
            var body = System.Text.Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "test.exchange",
                routingKey: "test.queue",
                basicProperties: null,
                body: body);

            _logger.LogInformation("RabbitMQ test mesajı gönderildi: {Message}", message);

            return Ok(new { message = "RabbitMQ test başarılı", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ test hatası");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpGet("logs")]
    public IActionResult GetLogs()
    {
        _logger.LogInformation("Log test endpoint çağrıldı");
        
        return Ok(new { 
            message = "Log test başarılı", 
            timestamp = DateTime.UtcNow,
            correlationId = HttpContext.TraceIdentifier 
        });
    }
}
