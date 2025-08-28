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
    
    /// <summary>
    /// Türkçe yorum: Varsayılan imza metodu - implementasyon gerekmeden çalışır.
    /// Not: Gerçek imza akışı eklendiğinde bu default override edilebilir.
    /// </summary>
    /// <param name="data">İmzalanacak veri</param>
    /// <param name="ct">İptal token'ı</param>
    /// <returns>İmzalı veri</returns>
    public virtual Task<byte[]> SignDocument(byte[] data, System.Threading.CancellationToken ct = default)
    {
        // Türkçe yorum: Şimdilik no-op: veriyi aynen döner.
        return Task.FromResult(data);
    }
    
    /// <summary>
    /// Türkçe yorum: string veri için sarmalayıcı overload
    /// </summary>
    /// <param name="data">İmzalanacak string veri</param>
    /// <param name="ct">İptal token'ı</param>
    /// <returns>İmzalı veri</returns>
    public virtual Task<byte[]> SignDocument(string data, System.Threading.CancellationToken ct = default)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(data ?? string.Empty);
        return SignDocument(bytes, ct);
    }
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
