using WebApi.Contracts.Invoices;
using WebApi.Infrastructure.Validation;

namespace Invoice.Domain.Tests;

/// <summary>
/// Unit tests for FluentValidation validators
/// </summary>
public class ValidationTests
{
    private readonly CreateInvoiceRequestValidator _createInvoiceValidator;
    private readonly InvoiceQueryFilterValidator _queryFilterValidator;

    public ValidationTests()
    {
        _createInvoiceValidator = new CreateInvoiceRequestValidator();
        _queryFilterValidator = new InvoiceQueryFilterValidator();
    }

    [Fact]
    public void CreateInvoiceRequestValidator_ValidRequest_ShouldPass()
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            CustomerId = "test-customer-123",
            Amount = 100.50m,
            Description = "Test invoice description",
            Currency = "TRY"
        };

        // Act
        var result = _createInvoiceValidator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CreateInvoiceRequestValidator_InvalidCustomerId_ShouldFail(string customerId)
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            CustomerId = customerId ?? string.Empty,
            Amount = 100.50m,
            Description = "Test invoice description",
            Currency = "TRY"
        };

        // Act
        var result = _createInvoiceValidator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateInvoiceRequest.CustomerId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void CreateInvoiceRequestValidator_InvalidAmount_ShouldFail(decimal amount)
    {
        // Arrange
        var request = new CreateInvoiceRequest
        {
            CustomerId = "test-customer-123",
            Amount = amount,
            Description = "Test invoice description",
            Currency = "TRY"
        };

        // Act
        var result = _createInvoiceValidator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateInvoiceRequest.Amount));
    }


    [Fact]
    public void InvoiceQueryFilterValidator_ValidFilter_ShouldPass()
    {
        // Arrange
        var filter = new InvoiceQueryFilter
        {
            Page = 1,
            PageSize = 10,
            Sort = "createdAt asc",
            Filter = "status=Active"
        };

        // Act
        var result = _queryFilterValidator.Validate(filter);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void InvoiceQueryFilterValidator_InvalidPage_ShouldFail(int page)
    {
        // Arrange
        var filter = new InvoiceQueryFilter
        {
            Page = page,
            PageSize = 10,
            Sort = "createdAt asc",
            Filter = "status=Active"
        };

        // Act
        var result = _queryFilterValidator.Validate(filter);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceQueryFilter.Page));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(201)]
    public void InvoiceQueryFilterValidator_InvalidPageSize_ShouldFail(int pageSize)
    {
        // Arrange
        var filter = new InvoiceQueryFilter
        {
            Page = 1,
            PageSize = pageSize,
            Sort = "createdAt asc",
            Filter = "status=Active"
        };

        // Act
        var result = _queryFilterValidator.Validate(filter);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(InvoiceQueryFilter.PageSize));
    }

}
