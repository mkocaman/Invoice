using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Invoice.Infrastructure.Data;
using System.Text;
using System.Text.Json;

namespace Invoice.Tests.Integration;

/// <summary>
/// Minimal API testleri
/// </summary>
[Trait("Category", "Integration")]
public class MinimalApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">Web application factory</param>
    public MinimalApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Test veritabanı kullan
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<InvoiceDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<InvoiceDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
    }

    /// <summary>
    /// Health endpoint testi
    /// </summary>
    [Fact]
    public async Task Health_Endpoint_Returns_200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(200, (int)response.StatusCode);
    }

    /// <summary>
    /// Health ready endpoint testi
    /// </summary>
    [Fact]
    public async Task HealthReady_Endpoint_Returns_200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(200, (int)response.StatusCode);
    }

    /// <summary>
    /// Charge session endpoint testi
    /// </summary>
    [Fact]
    public async Task ChargeSession_Endpoint_Returns_202()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            eshuId = "test-eshu-123",
            startDate = DateTime.UtcNow,
            amount = 100.50m
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/invoices/charge-session", content);

        // Assert
        Assert.Equal(202, (int)response.StatusCode);
    }

    /// <summary>
    /// Webhook endpoint - geçersiz imza testi
    /// </summary>
    [Fact]
    public async Task Webhook_InvalidSignature_Returns_401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new { test = "data" };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Geçersiz header'lar
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks/integrators/foriba")
        {
            Content = content
        };
        request.Headers.Add("X-Signature", "invalid-signature");
        request.Headers.Add("X-Timestamp", DateTime.UtcNow.ToString("O"));

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(401, (int)response.StatusCode);
    }

    /// <summary>
    /// Webhook endpoint - eksik header testi
    /// </summary>
    [Fact]
    public async Task Webhook_MissingHeaders_Returns_401()
    {
        // Arrange
        var client = _factory.CreateClient();
        var payload = new { test = "data" };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Header yok
        var request = new HttpRequestMessage(HttpMethod.Post, "/webhooks/integrators/logo")
        {
            Content = content
        };

        // Act
        var response = await client.SendAsync(request);

        // Assert
        Assert.Equal(401, (int)response.StatusCode);
    }

    /// <summary>
    /// Rate limiting testi
    /// </summary>
    [Fact]
    public async Task RateLimit_ExceedsLimit_Returns_429()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act - Çok fazla istek gönder
        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 150; i++) // Limit 100/dk
        {
            tasks.Add(client.GetAsync("/api/invoices"));
        }

        var responses = await Task.WhenAll(tasks);

        // Assert - En az bir 429 yanıtı olmalı
        var has429 = responses.Any(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests);
        Assert.True(has429, "Rate limiting çalışmıyor");
    }
}
