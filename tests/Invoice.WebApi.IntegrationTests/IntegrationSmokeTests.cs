using WebApi.Contracts.Invoices;

namespace Invoice.WebApi.IntegrationTests;

/// <summary>
/// Integration smoke tests to verify API endpoints work correctly
/// </summary>
public class IntegrationSmokeTests : IClassFixture<InvoiceApiFactory>
{
    private readonly InvoiceApiFactory _factory;

    public IntegrationSmokeTests(InvoiceApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthCheck_Live_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task HealthCheck_Ready_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Swagger_ShouldBeAccessible()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerJson_ShouldBeAccessible()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("swagger");
    }

    [Fact]
    public async Task InvoiceExamples_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/invoice-examples");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvoiceExamples_WithInvalidToken_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateAuthenticatedClient("invalid-token");

        // Act
        var response = await client.GetAsync("/api/invoice-examples");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvoiceExamples_WithTokenWithoutScope_ShouldReturnForbidden()
    {
        // Arrange
        var token = JwtTestHelper.CreateTestTokenWithoutScope();
        using var client = _factory.CreateAuthenticatedClient(token);

        // Act
        var response = await client.GetAsync("/api/invoice-examples");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InvoiceExamples_WithValidCsmsToken_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateCsmsAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/invoice-examples");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify pagination headers are present
        response.Headers.Should().ContainKey("X-Pagination");
        response.Headers.Should().ContainKey("X-Total-Count");
        response.Headers.Should().ContainKey("X-Page-Size");
        response.Headers.Should().ContainKey("X-Current-Page");
    }

    [Fact]
    public async Task InvoiceExamples_WithPagination_ShouldReturnCorrectHeaders()
    {
        // Arrange
        using var client = _factory.CreateCsmsAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/invoice-examples?pageNumber=2&pageSize=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify pagination headers
        response.Headers.GetValues("X-Current-Page").First().Should().Be("2");
        response.Headers.GetValues("X-Page-Size").First().Should().Be("5");
    }

    [Fact]
    public async Task CreateInvoice_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        using var client = _factory.CreateCsmsAuthenticatedClient();
        var request = new CreateInvoiceRequest
        {
            CustomerId = "test-customer-123",
            Amount = 150.75m,
            Description = "Integration test invoice",
            Currency = "TRY"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/invoice-examples", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateInvoice_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        using var client = _factory.CreateCsmsAuthenticatedClient();
        var request = new CreateInvoiceRequest
        {
            CustomerId = "", // Invalid: empty customer ID
            Amount = -10, // Invalid: negative amount
            Description = "Test invoice",
            Currency = "TRY"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/invoice-examples", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("validation");
    }

    [Fact]
    public async Task CreateInvoice_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new CreateInvoiceRequest
        {
            CustomerId = "test-customer-123",
            Amount = 150.75m,
            Description = "Test invoice",
            Currency = "TRY"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/invoice-examples", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TestController_Get_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/test");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Test endpoint");
    }

    [Fact]
    public async Task TestController_GetWithAuth_ShouldReturnOk()
    {
        // Arrange
        using var client = _factory.CreateCsmsAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/test/auth");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Authenticated");
    }
}
