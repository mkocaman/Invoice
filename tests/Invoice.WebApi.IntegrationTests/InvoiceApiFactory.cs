using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Invoice.WebApi.IntegrationTests;

/// <summary>
/// Custom WebApplicationFactory for integration tests
/// Configures the test host with test-specific settings and database
/// </summary>
public class InvoiceApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private string? _testConnectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database context
            _testConnectionString = "Host=localhost;Database=invoice_test;Username=postgres;Password=postgres;Port=5432";
            services.AddDbContext<DbContext>(options =>
            {
                options.UseNpgsql(_testConnectionString);
            });
        });

        // Use test configuration
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("config/appsettings.Test.json", optional: false, reloadOnChange: true);
        });

        // Disable logging for cleaner test output
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }

    public async Task InitializeAsync()
    {
        // Ensure test database exists and apply migrations
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        
        try
        {
            // Create database if it doesn't exist
            await context.Database.EnsureCreatedAsync();
            
            // Apply any pending migrations
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the test setup
            // This allows tests to run even if database setup fails
            Console.WriteLine($"Database setup warning: {ex.Message}");
        }
    }

    public new async Task DisposeAsync()
    {
        // Clean up test database if needed
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DbContext>();
        
        try
        {
            // Optionally clean up test data
            // await context.Database.EnsureDeletedAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database cleanup warning: {ex.Message}");
        }
    }

    /// <summary>
    /// Creates an HttpClient with a valid test JWT token
    /// </summary>
    /// <param name="token">JWT token to use for authentication</param>
    /// <returns>HttpClient with Authorization header set</returns>
    public HttpClient CreateAuthenticatedClient(string? token = null)
    {
        var client = CreateClient();
        
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        
        return client;
    }

    /// <summary>
    /// Creates an HttpClient with a valid CSMS test token
    /// </summary>
    /// <returns>HttpClient with valid CSMS JWT token</returns>
    public HttpClient CreateCsmsAuthenticatedClient()
    {
        var token = JwtTestHelper.CreateTestToken();
        return CreateAuthenticatedClient(token);
    }
}
