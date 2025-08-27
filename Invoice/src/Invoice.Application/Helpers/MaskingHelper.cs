namespace Invoice.Application.Helpers;

/// <summary>
/// PII (Personally Identifiable Information) maskeleme yardımcı sınıfı
/// </summary>
public static class MaskingHelper
{
    /// <summary>
    /// Plaka numarasını maskeler
    /// </summary>
    /// <param name="plate">Plaka numarası</param>
    /// <returns>Maskelenmiş plaka</returns>
    public static string MaskPlate(string? plate)
    {
        if (string.IsNullOrEmpty(plate) || plate.Length < 3)
            return plate ?? string.Empty;
        
        // İlk 2 ve son 1 karakteri göster, ortadakileri maskele
        return $"{plate.Substring(0, 2)}***{plate.Substring(plate.Length - 1)}";
    }
    
    /// <summary>
    /// VKN/TCKN numarasını maskeler
    /// </summary>
    /// <param name="vkn">VKN/TCKN numarası</param>
    /// <returns>Maskelenmiş VKN/TCKN</returns>
    public static string MaskVkn(string? vkn)
    {
        if (string.IsNullOrEmpty(vkn) || vkn.Length < 4)
            return vkn ?? string.Empty;
        
        // İlk 3 ve son 1 karakteri göster, ortadakileri maskele
        return $"{vkn.Substring(0, 3)}****{vkn.Substring(vkn.Length - 1)}";
    }
    
    /// <summary>
    /// Email adresini maskeler
    /// </summary>
    /// <param name="email">Email adresi</param>
    /// <returns>Maskelenmiş email</returns>
    public static string MaskEmail(string? email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            return email ?? string.Empty;
        
        var parts = email.Split('@');
        if (parts.Length != 2)
            return email;
        
        var username = parts[0];
        var domain = parts[1];
        
        if (username.Length <= 2)
            return email;
        
        var maskedUsername = $"{username.Substring(0, 1)}***{username.Substring(username.Length - 1)}";
        return $"{maskedUsername}@{domain}";
    }
    
    /// <summary>
    /// Telefon numarasını maskeler
    /// </summary>
    /// <param name="phone">Telefon numarası</param>
    /// <returns>Maskelenmiş telefon</returns>
    public static string MaskPhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 6)
            return phone ?? string.Empty;
        
        // İlk 3 ve son 2 karakteri göster, ortadakileri maskele
        return $"{phone.Substring(0, 3)}****{phone.Substring(phone.Length - 2)}";
    }
}
