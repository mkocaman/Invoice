namespace Invoice.Domain.Enums;

/// <summary>
/// İmzalama modları
/// </summary>
public enum SigningMode
{
    /// <summary>
    /// Dijital imza
    /// </summary>
    DIGITAL = 1,
    
    /// <summary>
    /// Fiziksel imza
    /// </summary>
    PHYSICAL = 2
}
