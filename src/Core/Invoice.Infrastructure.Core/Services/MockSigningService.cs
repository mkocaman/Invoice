using Microsoft.Extensions.Logging;
using Invoice.Application.Interfaces;

namespace Invoice.Infrastructure.Services;

/// <summary>
/// Mock imzalama servisi - dev ortamında kullanılır
/// </summary>
public class MockSigningService : ISigningService
{
    private readonly ILogger<MockSigningService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public MockSigningService(ILogger<MockSigningService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// UBL XML'i XAdES-BES ile imzalar (mock)
    /// </summary>
    public Task<string> SignUblAsync(string ublXml, string certificatePath, string certificatePassword)
    {
        _logger.LogInformation("Mock XAdES-BES imzalama başlatılıyor. Certificate: {CertificatePath}", certificatePath);

        // Mock imzalama - gerçek implementasyonda XAdES-BES imzalama yapılır
        // Simüle edilmiş gecikme kaldırıldı (gereksiz async)

        // Mock imzalı XML oluştur
        var signedXml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<SignedInvoice xmlns=""urn:oasis:names:specification:ubl:schema:xsd:Invoice-2"">
    <!-- Mock XAdES-BES imza -->
    <ds:Signature xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">
        <ds:SignedInfo>
            <ds:CanonicalizationMethod Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/>
            <ds:SignatureMethod Algorithm=""http://www.w3.org/2001/04/xmldsig-more#rsa-sha256""/>
            <ds:Reference URI="""">
                <ds:Transforms>
                    <ds:Transform Algorithm=""http://www.w3.org/2000/09/xmldsig#enveloped-signature""/>
                    <ds:Transform Algorithm=""http://www.w3.org/TR/2001/REC-xml-c14n-20010315""/>
                </ds:Transforms>
                <ds:DigestMethod Algorithm=""http://www.w3.org/2001/04/xmlenc#sha256""/>
                <ds:DigestValue>MOCK_DIGEST_VALUE</ds:DigestValue>
            </ds:Reference>
        </ds:SignedInfo>
        <ds:SignatureValue>MOCK_SIGNATURE_VALUE</ds:SignatureValue>
        <ds:KeyInfo>
            <ds:X509Data>
                <ds:X509Certificate>MOCK_CERTIFICATE_DATA</ds:X509Certificate>
            </ds:X509Data>
        </ds:KeyInfo>
    </ds:Signature>
    
    <!-- Orijinal UBL XML -->
    {ublXml}
</SignedInvoice>";

        _logger.LogInformation("Mock XAdES-BES imzalama tamamlandı. İmzalı XML boyutu: {Size} karakter", signedXml.Length);

        return Task.FromResult(signedXml);
    }

    /// <summary>
    /// İmza doğrulama (mock)
    /// </summary>
    public Task<bool> VerifySignatureAsync(string signedXml)
    {
        _logger.LogInformation("Mock imza doğrulama başlatılıyor");

        // Mock doğrulama - her zaman true döner
        // Simüle edilmiş gecikme kaldırıldı (gereksiz async)

        _logger.LogInformation("Mock imza doğrulama tamamlandı: Geçerli");

        return Task.FromResult(true);
    }

    /// <summary>
    /// Sertifika bilgilerini alır (mock)
    /// </summary>
    public Task<CertificateInfo> GetCertificateInfoAsync(string certificatePath, string certificatePassword)
    {
        _logger.LogInformation("Mock sertifika bilgileri alınıyor. Certificate: {CertificatePath}", certificatePath);

        // Mock sertifika bilgileri
        // Simüle edilmiş gecikme kaldırıldı (gereksiz async)

        var certificateInfo = new CertificateInfo
        {
            SerialNumber = "MOCK_SERIAL_123456789",
            Subject = "CN=Mock Test Certificate, O=Mock Organization, C=TR",
            ValidFrom = DateTime.Today.AddYears(-1),
            ValidTo = DateTime.Today.AddYears(1),
            CertificateType = "XAdES-BES"
        };

        _logger.LogInformation("Mock sertifika bilgileri alındı. Serial: {SerialNumber}", certificateInfo.SerialNumber);

        return Task.FromResult(certificateInfo);
    }
}
