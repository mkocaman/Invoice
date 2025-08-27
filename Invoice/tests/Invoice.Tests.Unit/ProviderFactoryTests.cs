using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Invoice.Application.Interfaces;
using Invoice.Infrastructure.Providers;
using Xunit;

namespace Invoice.Tests.Unit;

/// <summary>
/// ProviderFactory unit testleri
/// </summary>
public class ProviderFactoryTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<InvoiceProviderFactory>> _mockLogger;
    private readonly InvoiceProviderFactory _factory;

    public ProviderFactoryTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<InvoiceProviderFactory>>();
        _factory = new InvoiceProviderFactory(_mockServiceProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public void Resolve_WithValidProviderKey_ReturnsProvider()
    {
        // Arrange
        var mockProvider = new Mock<IInvoiceProvider>();
        mockProvider.Setup(p => p.Name).Returns("foriba");

        var services = new List<IInvoiceProvider> { mockProvider.Object };
        _mockServiceProvider.Setup(sp => sp.GetServices<IInvoiceProvider>())
            .Returns(services);

        // Act
        var result = _factory.Resolve("foriba");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("foriba", result.Name);
    }

    [Fact]
    public void Resolve_WithInvalidProviderKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockProvider = new Mock<IInvoiceProvider>();
        mockProvider.Setup(p => p.Name).Returns("foriba");

        var services = new List<IInvoiceProvider> { mockProvider.Object };
        _mockServiceProvider.Setup(sp => sp.GetServices<IInvoiceProvider>())
            .Returns(services);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _factory.Resolve("invalid"));
        Assert.Contains("invalid", exception.Message);
        Assert.Contains("foriba", exception.Message);
    }

    [Fact]
    public void Resolve_WithEmptyProviderKey_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Resolve(""));
        Assert.Contains("ProviderKey boş olamaz", exception.Message);
    }

    [Fact]
    public void Resolve_WithNullProviderKey_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Resolve(null!));
        Assert.Contains("ProviderKey boş olamaz", exception.Message);
    }

    [Fact]
    public void Resolve_WithWhitespaceProviderKey_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Resolve("   "));
        Assert.Contains("ProviderKey boş olamaz", exception.Message);
    }

    [Fact]
    public void Resolve_WithCaseInsensitiveProviderKey_ReturnsProvider()
    {
        // Arrange
        var mockProvider = new Mock<IInvoiceProvider>();
        mockProvider.Setup(p => p.Name).Returns("foriba");

        var services = new List<IInvoiceProvider> { mockProvider.Object };
        _mockServiceProvider.Setup(sp => sp.GetServices<IInvoiceProvider>())
            .Returns(services);

        // Act
        var result = _factory.Resolve("FORIBA");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("foriba", result.Name);
    }
}
