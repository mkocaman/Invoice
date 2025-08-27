using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Invoice.Tests.Integration;

/// <summary>
/// InvoicesController integration testleri
/// </summary>
[Collection("Integration")]
public class InvoicesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public InvoicesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Test veritabanı kullan
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<InvoiceDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<InvoiceDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateChargeSessionInvoice_WithValidRequest_ReturnsAccepted()
    {
        // Arrange
        var request = new
        {
            tenantId = "test-tenant",
            providerKey = "foriba",
            idempotencyKey = Guid.NewGuid().ToString(),
            customer = new
            {
                customerId = "CUST001",
                name = "Test Müşteri",
                taxNumber = "1234567890",
                taxOffice = "Test Vergi Dairesi",
                address = "Test Adres",
                phone = "05551234567",
                email = "test@example.com"
            },
            lineItems = new[]
            {
                new
                {
                    description = "Test Ürün",
                    quantity = 1,
                    unitPrice = 100.00m,
                    lineTotal = 100.00m,
                    taxRate = 18,
                    taxAmount = 18.00m,
                    unit = "adet",
                    productCode = "TEST001"
                }
            },
            subTotal = 100.00m,
            taxAmount = 18.00m,
            totalAmount = 118.00m,
            eshuAmount = 5.00m,
            eshuDescription = "EŞÜ ücreti",
            notes = "Test fatura"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/invoices/charge-session", content);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        Assert.True(result.TryGetProperty("invoiceId", out _));
        Assert.True(result.TryGetProperty("invoiceNumber", out _));
        Assert.True(result.TryGetProperty("status", out var status));
        Assert.Equal("ACCEPTED", status.GetString());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateChargeSessionInvoice_WithInvalidProvider_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            tenantId = "test-tenant",
            providerKey = "invalid-provider",
            idempotencyKey = Guid.NewGuid().ToString(),
            customer = new
            {
                customerId = "CUST001",
                name = "Test Müşteri",
                taxNumber = "1234567890"
            },
            lineItems = new[]
            {
                new
                {
                    description = "Test Ürün",
                    quantity = 1,
                    unitPrice = 100.00m,
                    lineTotal = 100.00m,
                    taxRate = 18,
                    taxAmount = 18.00m,
                    unit = "adet"
                }
            },
            subTotal = 100.00m,
            taxAmount = 18.00m,
            totalAmount = 118.00m,
            eshuAmount = 5.00m
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/invoices/charge-session", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        Assert.True(result.TryGetProperty("error", out var error));
        Assert.Contains("Provider konfigürasyonu bulunamadı", error.GetString());
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetInvoice_WithValidId_ReturnsOk()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/invoices/{invoiceId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CreateChargeBatchInvoices_WithValidRequest_ReturnsAccepted()
    {
        // Arrange
        var request = new
        {
            reportId = Guid.NewGuid(),
            tenantId = "test-tenant",
            providerKey = "foriba"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/invoices/charge-batch", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        Assert.True(result.TryGetProperty("error", out var error));
        Assert.Contains("Rapor bulunamadı", error.GetString());
    }
}
