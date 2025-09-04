namespace Invoice.Infrastructure.UZ.Helpers;

/// <summary>
/// Özbekistan kimlik doğrulama helper'ı
/// </summary>
public static class UzIdHelper
{
    /// <summary>
    /// INN (Individual Taxpayer Number) validasyonu - 9 hane
    /// </summary>
    public static bool IsInn(string? inn)
    {
        if (string.IsNullOrWhiteSpace(inn))
            return false;
        
        return inn.Length == 9 && inn.All(char.IsDigit);
    }

    /// <summary>
    /// PINFL (Personal Identification Number) validasyonu - 14 hane
    /// </summary>
    public static bool IsPinfl(string? pinfl)
    {
        if (string.IsNullOrWhiteSpace(pinfl))
            return false;
        
        return pinfl.Length == 14 && pinfl.All(char.IsDigit);
    }

    /// <summary>
    /// Para birimi UZS kontrolü
    /// </summary>
    public static bool IsUzsCurrency(string? currency)
    {
        return string.Equals(currency, "UZS", StringComparison.OrdinalIgnoreCase);
    }
}
