namespace Invoice.Infrastructure.KZ.Helpers;

/// <summary>
/// Kazakistan kimlik doğrulama helper'ı
/// </summary>
public static class KzIdHelper
{
    /// <summary>
    /// BIN (Business Identification Number) validasyonu - 12 hane
    /// </summary>
    public static bool IsBin(string? bin)
    {
        if (string.IsNullOrWhiteSpace(bin))
            return false;
        
        return bin.Length == 12 && bin.All(char.IsDigit);
    }

    /// <summary>
    /// IIN (Individual Identification Number) validasyonu - 12 hane
    /// </summary>
    public static bool IsIin(string? iin)
    {
        if (string.IsNullOrWhiteSpace(iin))
            return false;
        
        return iin.Length == 12 && iin.All(char.IsDigit);
    }

    /// <summary>
    /// Para birimi KZT kontrolü
    /// </summary>
    public static bool IsKztCurrency(string? currency)
    {
        return string.Equals(currency, "KZT", StringComparison.OrdinalIgnoreCase);
    }
}
