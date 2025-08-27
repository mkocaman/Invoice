namespace Invoice.Application.Interfaces;

/// <summary>
/// UBL XML doğrulama sonucu
/// </summary>
public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// UBL XML doğrulama servisi sözleşmesi
/// </summary>
public interface IUblValidationService
{
    /// <summary>
    /// UBL XML'i şema ve iş kurallarına göre doğrular
    /// </summary>
    /// <param name="ublXml">UBL XML string</param>
    /// <param name="schemaVersion">Şema versiyonu</param>
    /// <returns>Doğrulama sonucu</returns>
    Task<ValidationResult> ValidateUblAsync(string ublXml, string schemaVersion = "2.1");
}
