namespace Invoice.Application.Interfaces;

/// <summary>
/// XAdES-BES imzalama servisi - SelfSign modunda kullanılır
/// </summary>
public interface ISigningService
{
    /// <summary>
    /// UBL XML'i XAdES-BES ile imzalar
    /// </summary>
    /// <param name="ublXml">İmzalanacak UBL XML</param>
    /// <param name="certificatePath">Sertifika dosya yolu</param>
    /// <param name="certificatePassword">Sertifika şifresi</param>
    /// <returns>İmzalı XML</returns>
    Task<string> SignUblAsync(string ublXml, string certificatePath, string certificatePassword);
    
    /// <summary>
    /// İmza doğrulama
    /// </summary>
    /// <param name="signedXml">İmzalı XML</param>
    /// <returns>İmza geçerli mi?</returns>
    Task<bool> VerifySignatureAsync(string signedXml);
    
    /// <summary>
    /// Sertifika bilgilerini alır
    /// </summary>
    /// <param name="certificatePath">Sertifika dosya yolu</param>
    /// <param name="certificatePassword">Sertifika şifresi</param>
    /// <returns>Sertifika bilgileri</returns>
    Task<CertificateInfo> GetCertificateInfoAsync(string certificatePath, string certificatePassword);
}

/// <summary>
/// Sertifika bilgileri
/// </summary>
public class CertificateInfo
{
    /// <summary>
    /// Sertifika seri numarası
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Sertifika sahibi
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Geçerlilik başlangıcı
    /// </summary>
    public DateTime ValidFrom { get; set; }
    
    /// <summary>
    /// Geçerlilik bitişi
    /// </summary>
    public DateTime ValidTo { get; set; }
    
    /// <summary>
    /// Sertifika türü
    /// </summary>
    public string CertificateType { get; set; } = string.Empty;
}
