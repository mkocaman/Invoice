using Invoice.Application.Helpers;

namespace Invoice.Tests.Unit;

/// <summary>
/// MaskingHelper unit testleri
/// </summary>
public class MaskingHelperTests
{
    /// <summary>
    /// Plaka maskeleme testi
    /// </summary>
    [Fact]
    public void MaskPlate_ValidPlate_ReturnsMasked()
    {
        // Arrange
        var plate = "34ABC123";

        // Act
        var result = MaskingHelper.MaskPlate(plate);

        // Assert
        Assert.Equal("34***3", result);
    }

    /// <summary>
    /// Kısa plaka maskeleme testi
    /// </summary>
    [Fact]
    public void MaskPlate_ShortPlate_ReturnsOriginal()
    {
        // Arrange
        var plate = "34";

        // Act
        var result = MaskingHelper.MaskPlate(plate);

        // Assert
        Assert.Equal("34", result);
    }

    /// <summary>
    /// Null plaka maskeleme testi
    /// </summary>
    [Fact]
    public void MaskPlate_NullPlate_ReturnsEmpty()
    {
        // Act
        var result = MaskingHelper.MaskPlate(null);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// VKN maskeleme testi
    /// </summary>
    [Fact]
    public void MaskVkn_ValidVkn_ReturnsMasked()
    {
        // Arrange
        var vkn = "1234567890";

        // Act
        var result = MaskingHelper.MaskVkn(vkn);

        // Assert
        Assert.Equal("123****0", result);
    }

    /// <summary>
    /// Kısa VKN maskeleme testi
    /// </summary>
    [Fact]
    public void MaskVkn_ShortVkn_ReturnsOriginal()
    {
        // Arrange
        var vkn = "123";

        // Act
        var result = MaskingHelper.MaskVkn(vkn);

        // Assert
        Assert.Equal("123", result);
    }

    /// <summary>
    /// Email maskeleme testi
    /// </summary>
    [Fact]
    public void MaskEmail_ValidEmail_ReturnsMasked()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = MaskingHelper.MaskEmail(email);

        // Assert
        Assert.Equal("t***t@example.com", result);
    }

    /// <summary>
    /// Kısa email maskeleme testi
    /// </summary>
    [Fact]
    public void MaskEmail_ShortEmail_ReturnsOriginal()
    {
        // Arrange
        var email = "a@b.com";

        // Act
        var result = MaskingHelper.MaskEmail(email);

        // Assert
        Assert.Equal("a@b.com", result);
    }

    /// <summary>
    /// Geçersiz email maskeleme testi
    /// </summary>
    [Fact]
    public void MaskEmail_InvalidEmail_ReturnsOriginal()
    {
        // Arrange
        var email = "invalid-email";

        // Act
        var result = MaskingHelper.MaskEmail(email);

        // Assert
        Assert.Equal("invalid-email", result);
    }

    /// <summary>
    /// Telefon maskeleme testi
    /// </summary>
    [Fact]
    public void MaskPhone_ValidPhone_ReturnsMasked()
    {
        // Arrange
        var phone = "5551234567";

        // Act
        var result = MaskingHelper.MaskPhone(phone);

        // Assert
        Assert.Equal("555****67", result);
    }

    /// <summary>
    /// Kısa telefon maskeleme testi
    /// </summary>
    [Fact]
    public void MaskPhone_ShortPhone_ReturnsOriginal()
    {
        // Arrange
        var phone = "555";

        // Act
        var result = MaskingHelper.MaskPhone(phone);

        // Assert
        Assert.Equal("555", result);
    }

    /// <summary>
    /// Boş değer maskeleme testi
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void MaskMethods_EmptyValues_ReturnsEmpty(string input)
    {
        // Act & Assert
        Assert.Equal(string.Empty, MaskingHelper.MaskPlate(input));
        Assert.Equal(string.Empty, MaskingHelper.MaskVkn(input));
        Assert.Equal(string.Empty, MaskingHelper.MaskEmail(input));
        Assert.Equal(string.Empty, MaskingHelper.MaskPhone(input));
    }
}
